
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DTO;

public class CreateUserDto
{
    [Required]
    [StringLength(100)]
    public required string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    public required string LastName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }

    [StringLength(100)]
    public string Roles { get; set; } = "basic";
}
