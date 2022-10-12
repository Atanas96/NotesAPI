using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesAPI.Data;
using NotesAPI.Models.Entities;

namespace NotesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private readonly NotesDbContext notesDbContext;
        public NotesController(NotesDbContext notesDbContext)
        {
            this.notesDbContext = notesDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            return Ok(await notesDbContext.Notes.ToListAsync());
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [ActionName("GetNoteById")]
        public async Task<IActionResult> GetNoteById(Guid id)
        {
            var dbNote = await notesDbContext.Notes.FindAsync(id);

            if (dbNote == null) return NotFound("The note is not found. ");

            return Ok(dbNote);
        }

        [HttpPost]

        public async Task<IActionResult> AddNote(Note note)
        {
             note.Id = Guid.NewGuid();
            await notesDbContext.Notes.AddAsync(note);
            await notesDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateNote([FromRoute] Guid id, [FromBody] Note updatedNote)
        {
            var note = await notesDbContext.Notes.FindAsync(id);
            if (note == null) return NotFound("The note is not found. ");

            note.Title = updatedNote.Title;
            note.Description = updatedNote.Description;
            note.IsVisible = updatedNote.IsVisible;

            await notesDbContext.SaveChangesAsync();
            return Ok(note);
        }

        [HttpDelete]
        [Route("{id:Guid}")]

        public async Task<IActionResult> DeleteNote([FromRoute] Guid id)
        {
            var deleteNote = await notesDbContext.Notes.FindAsync(id);

            if (deleteNote == null) return NotFound("The note is not found. ");

            notesDbContext.Notes.Remove(deleteNote);
            await notesDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
