
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DTO;

public class UpdateUserDto
{
    [StringLength(100)]
    public string? FirstName { get; set; }
    
    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }

    [StringLength(100)]
    public string? Roles { get; set; }
}