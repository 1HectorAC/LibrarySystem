
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DTO;

public class CheckoutDto
{
    [Required]
    public int UserId;

    [Required]
    public int BookCopyId;

}