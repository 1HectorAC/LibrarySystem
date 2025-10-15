
using LibrarySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Data;

public class LibrarySystem: DbContext
{
    public LibrarySystem(DbContextOptions<LibrarySystem> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        

    }
    DbSet<Author> Authors { get; set; }
    DbSet<Book> Books { get; set; }
    DbSet<BookCopy> BookCopies { get; set; }
    DbSet<Checkout> Checkouts { get; set; }
    DbSet<User> Users { get; set; }
}