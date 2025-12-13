
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DTO;

public class CheckoutDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int BookCopyId { get; set; }

}