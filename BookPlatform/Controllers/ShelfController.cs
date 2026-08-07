using BookPlatform.Data;
using BookPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookPlatform.Controllers
{
    [Authorize]
    public class ShelfController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShelfController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var items = await _context.ShelfItems
                .Where(s => s.UserId == userId)
                .Include(s => s.Book!)
                    .ThenInclude(b => b.Reviews)
                .OrderByDescending(s => s.AddedAt)
                .ToListAsync();

            var books = items.Select(i => i.Book!).ToList();
            return View(books);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int bookId)
        {
            var userId = _userManager.GetUserId(User)!;
            var exists = await _context.ShelfItems.AnyAsync(s => s.UserId == userId && s.BookId == bookId);
            if (!exists)
            {
                _context.ShelfItems.Add(new ShelfItem { UserId = userId, BookId = bookId });
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true, onShelf = true });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int bookId)
        {
            var userId = _userManager.GetUserId(User)!;
            var item = await _context.ShelfItems.FirstOrDefaultAsync(s => s.UserId == userId && s.BookId == bookId);
            if (item != null)
            {
                _context.ShelfItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true, onShelf = false });
        }
    }
}
