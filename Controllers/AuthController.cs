
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibrarySystem.Data;
using LibrarySystem.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibrarySystem.Controllers;


[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly LibraryDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(LibraryDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// Login endpoint for users with admin or employee roles.
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == loginDto.Email);
        
        // Validate user
        if (user is null)
        {
            return Unauthorized("Invalid email.");
        }
        if(!user.Roles.Contains("admin") || !user.Roles.Contains("employee"))
        {
            return Unauthorized("Invalid user role.");
        }
        if(BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password) == false)
        {
            return Unauthorized("Invalid password.");
        }

        // Create Claims
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };
        var roles = user.Roles.Split(',');
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
        }
        
        // Setup Key and Credentials
        var Secret_key = _configuration["SECRET_KEY"] ?? throw new InvalidOperationException("Secret key not found in configuration. Controller: AuthController, Method: Login");
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Secret_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the Token
        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        return Ok(new { Token = tokenString });
    }
}