
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("genre/[action]")]
[ApiController]
public class GenreController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public GenreController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Genre>>> Genres()
    {
        var genres = await _context.Genres.ToListAsync();
        return Ok(genres);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Genre>> Genre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre is null)
            return NotFound(new { Message = $"Genre with Id {id} not found" });
        return Ok(genre);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        var genre = await _context.Genres.FindAsync(id);

        if (genre is null)
            return NotFound(new { Message = $"Genre with Id {id} not found" });

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<Genre>> UpdateGenre(int id, [FromBody] Genre updatedGenre)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre is null)
            return NotFound(new { Messasge = $"Genre with Id {id} not found." });

        genre.Name = updatedGenre.Name;
        genre.Description = updatedGenre.Description;

        await _context.SaveChangesAsync();

        return Ok(genre);
    }

    [HttpPost]
    public async Task<ActionResult<Genre>> AddGenre([FromBody] Genre genre)
    {
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Genre), new { id = genre.Id }, genre);
    }

}