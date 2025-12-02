
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DTO;

public class LoginDto
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }
}