using BookPlatform.Data;
using BookPlatform.Models;
using BookPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookPlatform.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index(string? q, string? genre, string? sort)
        {
            var query = _context.Books.Include(b => b.Reviews).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(b => b.Title.Contains(q) || b.Author.Contains(q));

            if (!string.IsNullOrWhiteSpace(genre) && genre != "Hamısı")
                query = query.Where(b => b.Genre == genre);

            var books = await query.ToListAsync();

            books = sort switch
            {
                "title" => books.OrderBy(b => b.Title).ToList(),
                "year" => books.OrderByDescending(b => b.Year).ToList(),
                "reviews" => books.OrderByDescending(b => b.ReviewCount).ToList(),
                _ => books.OrderByDescending(b => b.AverageRating).ToList(),
            };

            ViewBag.Genres = await _context.Books.Select(b => b.Genre).Distinct().OrderBy(g => g).ToListAsync();
            ViewBag.CurrentGenre = string.IsNullOrWhiteSpace(genre) ? "Hamısı" : genre;
            ViewBag.CurrentQuery = q ?? "";
            ViewBag.CurrentSort = sort ?? "rating";

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.ShelfIds = await _context.ShelfItems
                    .Where(s => s.UserId == userId)
                    .Select(s => s.BookId)
                    .ToListAsync();
            }
            else
            {
                ViewBag.ShelfIds = new List<int>();
            }

            return View(books);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            bool onShelf = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User);
                onShelf = await _context.ShelfItems.AnyAsync(s => s.UserId == userId && s.BookId == id);
            }
            ViewBag.OnShelf = onShelf;

            return View(book);
        }

        
        public async Task<IActionResult> Read(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.ReadCount += 1;
            await _context.SaveChangesAsync();

            return View(book);
        }

        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewVM model)
        {
            if (model.Rating < 1 || model.Rating > 5 || string.IsNullOrWhiteSpace(model.Text))
                return BadRequest(new { message = "Ulduz seç və rəy mətnini daxil et." });

            var book = await _context.Books.Include(b => b.Reviews).FirstOrDefaultAsync(b => b.Id == model.BookId);
            if (book == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var existing = book.Reviews.FirstOrDefault(r => r.UserId == user.Id);
            if (existing != null)
            {
                existing.Rating = model.Rating;
                existing.Text = model.Text;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Reviews.Add(new Review
                {
                    BookId = book.Id,
                    UserId = user.Id,
                    UserName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.Email ?? "İstifadəçi" : user.DisplayName,
                    Rating = model.Rating,
                    Text = model.Text
                });
            }

            await _context.SaveChangesAsync();

            var updated = await _context.Books.Include(b => b.Reviews).FirstAsync(b => b.Id == model.BookId);

            return Json(new
            {
                success = true,
                averageRating = Math.Round(updated.AverageRating, 1),
                reviewCount = updated.ReviewCount,
                reviews = updated.Reviews.OrderByDescending(r => r.CreatedAt).Select(r => new
                {
                    r.UserName,
                    r.Rating,
                    r.Text
                })
            });
        }

        
        [Authorize]
        public async Task<IActionResult> Recommendations()
        {
            var userId = _userManager.GetUserId(User);

            var likedGenres = await _context.Reviews
                .Where(r => r.UserId == userId && r.Rating >= 4)
                .Include(r => r.Book)
                .Select(r => r.Book!.Genre)
                .ToListAsync();

            List<Book> recs;
            string note;

            if (likedGenres.Count == 0)
            {
                recs = await _context.Books.Include(b => b.Reviews).ToListAsync();
                recs = recs.OrderByDescending(b => b.AverageRating).Take(6).ToList();
                note = "Hələ heç bir kitaba 4-5 ulduz verməmisən — ən yüksək qiymətli kitablar göstərilir.";
            }
            else
            {
                var topGenre = likedGenres
                    .GroupBy(g => g)
                    .OrderByDescending(g => g.Count())
                    .First().Key;

                var reviewedIds = await _context.Reviews.Where(r => r.UserId == userId).Select(r => r.BookId).ToListAsync();

                recs = await _context.Books
                    .Include(b => b.Reviews)
                    .Where(b => b.Genre == topGenre && !reviewedIds.Contains(b.Id))
                    .ToListAsync();
                recs = recs.OrderByDescending(b => b.AverageRating).Take(6).ToList();
                note = $"\"{topGenre}\" janrına yüksək qiymət verdiyinə görə bunları bəyənə bilərsən:";
            }

            ViewBag.Note = note;
            return View(recs);
        }
    }
}
