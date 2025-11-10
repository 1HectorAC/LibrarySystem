
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("api/publishers")]
[ApiController]
public class PublisherController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public PublisherController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Publisher>>> GetPublishers()
    {
        var publishers = await _context.Publishers.ToListAsync();
        return Ok(publishers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Publisher>> GetPublisher(int id)
    {
        var publisher = await _context.Publishers.FirstOrDefaultAsync(i => i.Id == id);
        if (publisher is null)
            return NotFound(new { Message = $"Publisher with Id {id} not found." });
        return Ok(publisher);
    }

    [HttpPost]
    public async Task<ActionResult<Publisher>> AddPublisher([FromBody] Publisher publisher)
    {
        _context.Add(publisher);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Publisher), new { id = publisher.Id }, publisher);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Publisher>> UpdatePublisher(int id, [FromBody] Publisher updatedPublisher)
    {
        var publisher = await _context.Publishers.FirstOrDefaultAsync(i => i.Id == id);
        if (publisher is null)
            return NotFound(new { Message = $"Publisher with id {id} not found." });

        publisher.Name = updatedPublisher.Name;
        publisher.Email = updatedPublisher.Email;

        await _context.SaveChangesAsync();
        return Ok(publisher);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePublisher(int id)
    {
        // Consider checking if books with this publisher exits before deleting.

        var publisher = await _context.Publishers.FirstOrDefaultAsync(i => i.Id == id);
        if (publisher is null)
            return NotFound(new { Message = $"Publisher with id {id} not found." });

        _context.Publishers.Remove(publisher);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}