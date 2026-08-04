using Microsoft.AspNetCore.Mvc;
using BookReview.Models;
using System.Text.Json;
using System.Net.Http;
using BookReview; 

namespace BookReview.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(string searchQuery)
        {
            List<Book> books = new List<Book>();

            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(6);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                    // Gutendex API - Limitsiz və Tam Açıq Kitab Bazası
                    string url = string.IsNullOrWhiteSpace(searchQuery)
                        ? "https://gutendex.com/books/"
                        : $"https://gutendex.com/books/?search={Uri.EscapeDataString(searchQuery)}";

                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        using (JsonDocument doc = JsonDocument.Parse(jsonString))
                        {
                            JsonElement root = doc.RootElement;
                            if (root.TryGetProperty("results", out JsonElement results) && results.ValueKind == JsonValueKind.Array)
                            {
                                int tempId = 1;
                                foreach (JsonElement item in results.EnumerateArray())
                                {
                                    if (tempId > 12) break; // Maksimum 12 kitab göstərsin

                                    // Başlıq
                                    string title = item.TryGetProperty("title", out JsonElement titleElem) ? titleElem.GetString() ?? "Adsız Kitab" : "Adsız Kitab";

                                    // Müəllif
                                    string authorName = "Naməlum Müəllif";
                                    if (item.TryGetProperty("authors", out JsonElement authorsElem) && authorsElem.ValueKind == JsonValueKind.Array)
                                    {
                                        var firstAuth = authorsElem.EnumerateArray().FirstOrDefault();
                                        if (firstAuth.ValueKind == JsonValueKind.Object && firstAuth.TryGetProperty("name", out JsonElement nameElem))
                                        {
                                            authorName = nameElem.GetString() ?? "Naməlum Müəllif";
                                        }
                                    }

                                    // Şəkil
                                    string imageUrl = "https://images.unsplash.com/photo-1543002588-bfa74002ed7e?w=400";
                                    if (item.TryGetProperty("formats", out JsonElement formatsElem))
                                    {
                                        if (formatsElem.TryGetProperty("image/jpeg", out JsonElement imgElem))
                                        {
                                            imageUrl = imgElem.GetString() ?? imageUrl;
                                        }
                                    }

                                    books.Add(new Book
                                    {
                                        Id = tempId++,
                                        Title = title,
                                        Description = "Bu əsər dünya klassik ədəbiyyatı fondundan daxil edilmişdir.",
                                        CoverImageUrl = imageUrl,
                                        Author = new Author { Name = authorName },
                                        Genre = new Genre { Name = "Klassika" }
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ApiDebug = $"API Xətası: {ex.Message}";
            }

            // Əgər API cavab verməzsə, ehtiyat kitablarımız işə düşür
            if (books.Count == 0)
            {
                books = GetFallbackBooks();
            }

            return View(books);
        }

        private List<Book> GetFallbackBooks()
        {
            return new List<Book>
            {
                new Book
                {
                    Id = 101,
                    Title = "Əli və Nino",
                    Description = "Şərq və Qərb mədəniyyətlərinin kəsişməsində cərəyan edən məhəbbət hekayəsi.",
                    CoverImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=400",
                    Author = new Author { Name = "Qurban Səid" },
                    Genre = new Genre { Name = "Bədii" }
                },
                new Book
                {
                    Id = 102,
                    Title = "1984",
                    Description = "Totalitar rejimin insan psixologiyası üzərindəki nəzarətini təsvir edən antiutopik roman.",
                    CoverImageUrl = "https://images.unsplash.com/photo-1543002588-bfa74002ed7e?w=400",
                    Author = new Author { Name = "George Orwell" },
                    Genre = new Genre { Name = "Antiutopiya" }
                }
            };
        }

        public async Task<IActionResult> Details(int id, string title, string author, string coverUrl, string description)
        {
            // Kliklənən kitabın məlumatlarını View-ya ötürürük
            var book = new Book
            {
                Id = id,
                Title = string.IsNullOrEmpty(title) ? "Adsız Kitab" : title,
                Description = string.IsNullOrEmpty(description) ? "Bu əsər haqqında geniş məlumat yaxın zamanda əlavə olunacaq." : description,
                CoverImageUrl = string.IsNullOrEmpty(coverUrl) ? "https://images.unsplash.com/photo-1543002588-bfa74002ed7e?w=400" : coverUrl,
                Author = new Author { Name = string.IsNullOrEmpty(author) ? "Naməlum Müəllif" : author },
                Genre = new Genre { Name = "Klassika" }
            };

            return View(book);
        }
        [HttpPost]
        public IActionResult AddReview(Review review)
        {
            // Sizin modelinizdə UserId olduğu üçün müvəqqəti və ya aktiv istifadəçi ID-sini mənimsədirik:
            review.UserId = 1; // İstifadəçi giriş edibsə: int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            review.CreatedAt = DateTime.Now;

            // Burada DB-yə əlavə etmə məntiqi (və ya DbContext) çağırıla bilər
            // _context.Reviews.Add(review);
            // _context.SaveChanges();

            TempData["SuccessMessage"] = "Rəyiniz uğurla əlavə olundu!";

            // Yenidən həmin kitabın Details səhifəsinə qaytarırıq
            return RedirectToAction("Details", new { id = review.BookId });
        }

        // 1. RƏFƏ ƏLAVƏ ET
        [HttpPost]
        public IActionResult AddToShelf(int bookId, string shelfType, string title, string coverUrl, string author)
        {
            // Müvəqqəti olaraq (və ya Session/Database istifadə edərək) kitabı saxlayırıq:
            var myShelf = HttpContext.Session.GetObject<List<ShelfItem>>("UserShelf") ?? new List<ShelfItem>();

            // Əgər kitab artıq rəfdə varsa, statusunu yeniləyirik
            var existingItem = myShelf.FirstOrDefault(s => s.BookId == bookId);
            if (existingItem != null)
            {
                existingItem.ShelfType = shelfType;
            }
            else
            {
                myShelf.Add(new ShelfItem
                {
                    BookId = bookId,
                    Title = title,
                    Author = author,
                    CoverUrl = coverUrl,
                    ShelfType = shelfType,
                    AddedDate = DateTime.Now
                });
            }

            // Sessiyada və ya TempData-da saxlayırıq
            TempData["ShelfMessage"] = "Kitab rəfə uğurla əlavə olundu!";
            return RedirectToAction("MyShelf");
        }

        // 2. RƏFİ GÖSTƏR SƏHİFƏSİ
        public IActionResult MyShelf()
        {
            // Rəfdəki kitabları gətiririk (Sessiyadan və ya Bazadan)
            var myShelf = HttpContext.Session.GetObject<List<ShelfItem>>("UserShelf") ?? new List<ShelfItem>();

            return View(myShelf);
        }
    }
}