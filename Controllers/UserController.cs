
using LibrarySystem.Data;
using LibrarySystem.DTO;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Authorize]
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

    // Update an existing user, Note: Only allows admin to update roles and password
    [HttpPut("{id}")]
    public async Task<ActionResult<Author>> UpdateUser(int id, [FromBody] UpdateUserDto updatedUser)
    {
        var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);
        if (user is null)
            return NotFound(new { Message = $"User with id {id} not found." });

        if(updatedUser.FirstName is not null)
            user.FirstName = updatedUser.FirstName;
        if(updatedUser.LastName is not null)
            user.LastName = updatedUser.LastName;
        if(updatedUser.Email is not null)
            user.Email = updatedUser.Email;
        
        // Admin check to ensure only admin can update roles and password
        if(updatedUser.Roles is not null && User.IsInRole("admin"))
            user.Roles = updatedUser.Roles;
        if(updatedUser.Password is not null && User.IsInRole("admin"))
            user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);


        await _context.SaveChangesAsync();

        return Ok(user);
    }

    // Add a new user, Note: Only allows admin to set roles
    [HttpPost]
    public async Task<ActionResult<User>> AddUser([FromBody] CreateUserDto user)
    {
        var newUser = new User
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
            // Admin check to ensure only admin can set roles
            Roles = User.IsInRole("admin") ? user.Roles : "basic"
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
    }

}