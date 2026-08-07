using BookPlatform.Data;
using BookPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Reviews).ToListAsync();

            ViewBag.TotalBooks = books.Count;
            ViewBag.TotalReviews = books.Sum(b => b.ReviewCount);
            var allRatings = books.SelectMany(b => b.Reviews.Select(r => r.Rating)).ToList();
            ViewBag.AvgRating = allRatings.Count > 0 ? Math.Round(allRatings.Average(), 1) : 0;

            var shelf = books.OrderByDescending(b => b.AverageRating).Take(14).ToList();
            return View(shelf);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
