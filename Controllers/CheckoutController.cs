
using LibrarySystem.Data;
using LibrarySystem.DTO;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("api/checkouts")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public CheckoutController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Checkout>>> GetCheckouts()
    {
        var checkouts = await _context.Checkouts.AsNoTracking().ToListAsync();
        return Ok(checkouts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Checkout>> GetCheckout(int id)
    {
        var checkout = await _context.Checkouts.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        if (checkout is null) 
            return NotFound();
            
        return Ok(checkout);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCheckout(int id)
    {
        var checkout = await _context.Checkouts.FirstOrDefaultAsync(i => i.Id == id);
        if (checkout is null)
            return NotFound(new { Message = $"Checkout with id {id} not found." });

        _context.Checkouts.Remove(checkout);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Author>> UpdateCheckout(int id, [FromBody] Checkout updatedCheckout)
    {
        var checkout = await _context.Checkouts.FirstOrDefaultAsync(a => a.Id == id);
        if (checkout is null)
            return NotFound(new { Message = $"Checkout with id {id} not found." });

        // Validate User exists
        bool userCheck = _context.Users.Any(i => i.Id == updatedCheckout.UserId);
        if (!userCheck)
            return BadRequest(new { Message = "UserId does not exits in db." });

        // Validate BookCopy exists
        bool bookCopyCheck = _context.BookCopies.Any(i => i.Id == updatedCheckout.BookCopyId);
        if (!bookCopyCheck)
            return BadRequest(new { Message = "BookCopyId does not exits in db." });

        checkout.UserId = updatedCheckout.UserId;
        checkout.BookCopyId = updatedCheckout.BookCopyId;
        checkout.CheckoutDate = updatedCheckout.CheckoutDate;
        checkout.DueDate = updatedCheckout.DueDate;
        checkout.DateReturned = updatedCheckout.DateReturned;

        await _context.SaveChangesAsync();

        return Ok(checkout);
    }

    [HttpPost]
    public async Task<ActionResult<Checkout>> AddCheckout([FromBody] CheckoutDto checkout)
    {
        var bookCopy = _context.BookCopies.FirstOrDefault(i => i.Id == checkout.BookCopyId);
        var UserCheck = _context.Users.Any(i => i.Id == checkout.UserId);

        // Validation (bookCopy, User, and BookCopy availability)
        if (bookCopy is null)
            return BadRequest(new { Message = "BookCopyId does not exists in db." });
        if (!UserCheck)
            return BadRequest(new { Message = "UserId does not exits in db." });
        if (!bookCopy.Available)
            return BadRequest(new { Message = "BookCopy is not available. Currently checked out already." });

        var result = new Checkout
        {
            BookCopyId = checkout.BookCopyId,
            UserId = checkout.UserId,
            CheckoutDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30)
        };

        _context.Checkouts.Add(result);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCheckout), new { id = result.Id }, result);
    }

    [HttpPut("{bookCopyId}/return-book")]
    public async Task<IActionResult> ReturnBook(int bookCopyId)
    {
        // There should only be one checkout with a BookCopy and no DateReturned,
        //  but consider adding Error warning if more exits.
        var checkout = await _context.Checkouts
            .Include(i => i.BookCopy)
            .FirstOrDefaultAsync(i => i.BookCopyId == bookCopyId && i.DateReturned == null);

        // Validation
        if (checkout is null)
            return NotFound(new { Message = $"No bookCopy with id {bookCopyId} is currently checkedout" });
        if (checkout.BookCopy is null)
            return BadRequest(new { Message = "Error getting the BookCopy" });
        if (checkout.BookCopy.Available)
            return BadRequest(new { Message = $"BookCopy with id {bookCopyId}, is already available, Can't checkin. Need to edit checkout if this is a system error" });

        checkout.DateReturned = DateTime.UtcNow;
        checkout.BookCopy.Available = true;
        await _context.SaveChangesAsync();
        return Ok(new { Message = $"BookCopy with id {bookCopyId} successfully checked-in" });
    }
}