using Microsoft.AspNetCore.Mvc;
using BookReview.Services;

namespace BookReview.Controllers
{
    public class HomeController : Controller
    {
        private readonly GoogleBooksService _googleBooksService;

        public HomeController(GoogleBooksService googleBooksService)
        {
            _googleBooksService = googleBooksService;
        }

        public async Task<IActionResult> Index(string searchQuery)
        {
            // İstifadəçi axtarış etməyibsə, ekrana populyar romanlar gəlsin
            string query = string.IsNullOrWhiteSpace(searchQuery) ? "bestsellers" : searchQuery;

            var books = await _googleBooksService.SearchBooksAsync(query);

            return View(books);
        }
    }
}