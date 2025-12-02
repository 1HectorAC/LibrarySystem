
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LibrarySystem.Models;

public class User
{
    public int Id { get; set; }

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
    [StringLength(100)]
    public required string Password {get; set;}

    [Required]
    [StringLength(100)]
    public required string Roles {get; set;}

    [JsonIgnore]
    public ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();

}