using BookPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookPlatform.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<ShelfItem> ShelfItems => Set<ShelfItem>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Book>()
                .HasMany(b => b.Reviews)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Book>()
                .HasMany(b => b.ShelfItems)
                .WithOne(s => s.Book)
                .HasForeignKey(s => s.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.Entity<ShelfItem>()
                .HasIndex(s => new { s.UserId, s.BookId })
                .IsUnique();
        }
    }
}
