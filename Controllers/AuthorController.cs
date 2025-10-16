
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("author/[action]")]
[ApiController]
public class AuthorController: ControllerBase
{
    private readonly LibraryDbContext _context;
    public AuthorController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Author>>> Authors()
    {

        return Ok(await _context.Authors.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Author>> Author(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(i => i.Id == id);
        if (author == null) return NotFound();
        return Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<Author>> Author([FromBody] Author author)
    {
        await _context.AddAsync(author);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Author), new {id=author.Id }, author);
    }

}