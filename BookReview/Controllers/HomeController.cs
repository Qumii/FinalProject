using System.Diagnostics;
using BookReview.Models;
using Microsoft.AspNetCore.Mvc;
using BookReview.Data;
using Microsoft.EntityFrameworkCore;

namespace BookReview.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? genreId)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .AsQueryable();

            if (genreId.HasValue)
            {
                query = query.Where(b => b.GenreId == genreId.Value);
            }

            var books = await query.ToListAsync();
            return View(books);
        }
    }
}
