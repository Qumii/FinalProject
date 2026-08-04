using BookReview.Data;
using BookReview.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookReview.Controllers
{
    [Authorize]
    public class UserShelfController : Controller
    {
        private readonly AppDbContext _context;

        public UserShelfController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Şəxsi Rəf Və Tövsiyələr Səhifəsi
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();

            // İstifadəçinin rəfə əlavə etdiyi kitablar
            var userShelves = await _context.UserShelves
                .Include(us => us.Book)
                .ThenInclude(b => b.Author)
                .Include(us => us.Book)
                .ThenInclude(b => b.Genre)
                .Where(us => us.UserId == userId)
                .ToListAsync();

            // --- TÖVSİYƏ ALQORİTMİ (RECOMMENDATION ENGINE) ---
            // 1. İstifadəçinin rəfində olan janrların ID-lərini tapırıq
            var userGenreIds = userShelves.Select(us => us.Book.GenreId).Distinct().ToList();

            // 2. İstifadəçinin artıq rəfinə əlavə etdiyi kitabların ID-ləri (onları təkrar tövsiyə etməyəcəyik)
            var userBookIds = userShelves.Select(us => us.BookId).ToList();

            // 3. Əgər istifadəçinin rəfində kitab varsa, onun bəyəndiyi janrlardan oxumadığı 4 kitabı tövsiyə edirik.
            //    Rəfi boşdursa, ən yüksək reytinqli/ümumi 4 kitabı təklif edirik.
            List<Book> recommendedBooks;

            if (userGenreIds.Any())
            {
                recommendedBooks = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Where(b => userGenreIds.Contains(b.GenreId) && !userBookIds.Contains(b.Id))
                    .Take(4)
                    .ToListAsync();
            }
            else
            {
                recommendedBooks = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Take(4)
                    .ToListAsync();
            }

            ViewBag.RecommendedBooks = recommendedBooks;

            return View(userShelves);
        }

        // 2. Kitabı Rəfə Əlavə Et və ya Statusunu Yenilə
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFromGoogle(string title, string authorName, string coverUrl, string description, ShelfStatus status)
        {
            int userId = GetCurrentUserId();

            // 1. Müəllifi tap və ya yarat
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
            if (author == null)
            {
                author = new Author { Name = string.IsNullOrWhiteSpace(authorName) ? "Naməlum Müəllif" : authorName };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
            }

            // 2. Kitabı bazada axtar, yoxdursa yarat
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == title && b.AuthorId == author.Id);
            if (book == null)
            {
                book = new Book
                {
                    Title = title,
                    Description = description ?? "Təsvir yoxdur.",
                    CoverImageUrl = coverUrl,
                    AuthorId = author.Id,
                    GenreId = 1 // Əsas/Ümumi janr ID-si
                };
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
            }

            // 3. Kitabı Şəxsi Rəfə (UserShelf) yaz
            var existingShelfItem = await _context.UserShelves
                .FirstOrDefaultAsync(us => us.UserId == userId && us.BookId == book.Id);

            if (existingShelfItem != null)
            {
                existingShelfItem.Status = status; // Varsa statusu yenilə
            }
            else
            {
                var shelfItem = new UserShelf
                {
                    UserId = userId,
                    BookId = book.Id,
                    Status = status
                };
                _context.UserShelves.Add(shelfItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "UserShelf");
        }

        // 3. Kitabı Rəfdən Sil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            int userId = GetCurrentUserId();
            var item = await _context.UserShelves.FirstOrDefaultAsync(us => us.Id == id && us.UserId == userId);

            if (item != null)
            {
                _context.UserShelves.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
