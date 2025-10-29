
using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers;

[Route("checkout/[action]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public CheckoutController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Checkout>>> Checkouts()
    {
        var checkouts = await _context.Checkouts.ToListAsync();
        return Ok(checkouts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Checkout>> Checkout(int id)
    {
        var checkout = await _context.Checkouts.FirstOrDefaultAsync(i => i.Equals(id));
        if (checkout is null) return NotFound();
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
    public async Task<ActionResult<Checkout>> Checkout([FromBody] Checkout checkout)
    {
        _context.Checkouts.Add(checkout);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Checkout), new {id = checkout.Id }, checkout);
    }
}