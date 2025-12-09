
using LibrarySystem.Data;
using LibrarySystem.DTO;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public UserController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        var users = await _context.Users.AsNoTracking().ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(i => i.Id == id);
        if (user is null)
            return NotFound(new { Message = $"User with id {id} not found." });

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Author>> UpdateUser(int id, [FromBody] User updatedUser)
    {
        var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
        if (user is null)
            return NotFound(new { Message = $"User with id {id} not found." });

        user.FirstName = updatedUser.FirstName;
        user.LastName = updatedUser.LastName;
        user.Email = updatedUser.Email;

        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> AddUser([FromBody] CreateUserDto user)
    {
        var newUser = new User
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
            Roles = user.Roles
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
    }

    // Limited to adding users with "basic" role only, preventing privilege escalation
    [HttpPost("basic")]
    public async Task<ActionResult<User>> AddBasicUser([FromBody] CreateUserDto user)
    {
        var newUser = new User
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
            Roles = "basic"
        };
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
    }

}