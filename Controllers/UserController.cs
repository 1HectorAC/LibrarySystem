
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("user/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public UserController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> Users()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(i => i.Id.Equals(id));
        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> MakeUser([FromBody] User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new {id = user.Id }, user);
    }
}