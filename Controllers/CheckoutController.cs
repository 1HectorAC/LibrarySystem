
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

    [HttpPost]
    public async Task<ActionResult<Checkout>> Checkout([FromBody] Checkout checkout)
    {
        _context.Checkouts.Add(checkout);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Checkout), new {id = checkout.Id }, checkout);
    }
}