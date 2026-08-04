using BookReview.Data;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Kitabın Detal Səhifəsi
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            // Orta balın avtomatik hesablanması
            double avgRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0.0;
            ViewBag.AverageRating = avgRating;

            return View(book);
        }

        // 2. Yeni Rəy və Ulduz Əlavə Et
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int bookId, int rating, string comment)
        {
            // Müvəqqəti olaraq UserId = 1 qoyuruq (Auth sistemini tamamlayanda dinamik edəcəyik)
            var review = new Review
            {
                BookId = bookId,
                UserId = 1,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = bookId });
        }
}
}
