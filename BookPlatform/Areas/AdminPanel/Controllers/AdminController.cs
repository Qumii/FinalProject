using BookPlatform.Data;
using BookPlatform.Models;
using BookPlatform.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookPlatform.Areas.AdminPanel.Controllers
{
    //[Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
        public class AdminController : Controller
        {
            private readonly AppDbContext _context;
            private readonly IWebHostEnvironment _env;


            public AdminController(AppDbContext context, IWebHostEnvironment env)
            {
                _context = context;
                _env = env;
            }

            
            public async Task<IActionResult> Index()
            {
                var books = await _context.Books
                    .Include(b => b.Reviews)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                ViewBag.TotalUsers = await _context.Users.CountAsync();
                ViewBag.TotalReviews = await _context.Reviews.CountAsync();

                return View(books);
            }

          
            public IActionResult Create()
            {
                return View(new BookVM { Year = DateTime.UtcNow.Year });
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(BookVM model)
            {
                if (!ModelState.IsValid) return View(model);

                var book = new Book
                {
                    Title = model.Title,
                    Author = model.Author,
                    Genre = model.Genre,
                    Year = model.Year,
                    Description = model.Description,
                    ContentPreview = string.IsNullOrWhiteSpace(model.ContentPreview)
                        ? $"Bu, \"{model.Title}\" kitabının nümunə oxu mətnidir (demo məqsədilə yazılmışdır)."
                        : model.ContentPreview,
                    CreatedAt = DateTime.UtcNow
                };

                book.CoverImageUrl = await SaveCoverAsync(model.CoverImage) ?? "/images/covers/default.svg";

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Kitab əlavə olundu.";
                return RedirectToAction(nameof(Index));
            }

            public async Task<IActionResult> Edit(int id)
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null) return NotFound();

                var model = new BookVM
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Genre = book.Genre,
                    Year = book.Year,
                    Description = book.Description,
                    ContentPreview = book.ContentPreview,
                    ExistingCoverImageUrl = book.CoverImageUrl
                };
                return View(model);
            }

            
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, BookVM model)
            {
                if (id != model.Id) return NotFound();

                var book = await _context.Books.FindAsync(id);
                if (book == null) return NotFound();

                if (!ModelState.IsValid)
                {
                    model.ExistingCoverImageUrl = book.CoverImageUrl;
                    return View(model);
                }

                book.Title = model.Title;
                book.Author = model.Author;
                book.Genre = model.Genre;
                book.Year = model.Year;
                book.Description = model.Description;
                if (!string.IsNullOrWhiteSpace(model.ContentPreview))
                    book.ContentPreview = model.ContentPreview;

                var newCover = await SaveCoverAsync(model.CoverImage);
                if (newCover != null)
                    book.CoverImageUrl = newCover;

                await _context.SaveChangesAsync();

                TempData["Message"] = "Kitab yeniləndi.";
                return RedirectToAction(nameof(Index));
            }

            
            public async Task<IActionResult> Delete(int id)
            {
                var book = await _context.Books.Include(b => b.Reviews).FirstOrDefaultAsync(b => b.Id == id);
                if (book == null) return NotFound();
                return View(book);
            }

            
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var book = await _context.Books.FindAsync(id);
                if (book != null)
                {
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Kitab silindi.";
                }
                return RedirectToAction(nameof(Index));
            }

            private async Task<string?> SaveCoverAsync(IFormFile? file)
            {
                if (file == null || file.Length == 0) return null;

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext)) return null;

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/uploads/{fileName}";
            }
        }
}
