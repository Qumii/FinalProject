using System.Text.Json;
using BookReview.Models;

namespace BookReview.Services
{
    public class GoogleBooksService
    {
        private readonly HttpClient _httpClient;

        public GoogleBooksService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(3); // Maksimum 3 saniyə gözləyir
        }

        public async Task<List<Book>> SearchBooksAsync(string query = "classic")
        {
            var books = new List<Book>();

            try
            {
                var search = string.IsNullOrWhiteSpace(query) ? "classic" : query;
                var url = $"https://openlibrary.org/search.json?q={Uri.EscapeDataString(search)}&limit=12";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonString);

                    if (doc.RootElement.TryGetProperty("docs", out var docs))
                    {
                        int idCounter = 1;
                        foreach (var docItem in docs.EnumerateArray())
                        {
                            string title = docItem.TryGetProperty("title", out var t) ? t.GetString() ?? "Adsız Kitab" : "Adsız Kitab";

                            string authorName = "Naməlum Müəllif";
                            if (docItem.TryGetProperty("author_name", out var authors) && authors.ValueKind == JsonValueKind.Array)
                            {
                                var firstAuth = authors.EnumerateArray().FirstOrDefault();
                                if (firstAuth.ValueKind == JsonValueKind.String)
                                {
                                    authorName = firstAuth.GetString()!;
                                }
                            }

                            string imageUrl = "https://images.unsplash.com/photo-1543002588-bfa74002ed7e?w=400";
                            if (docItem.TryGetProperty("cover_i", out var coverId) && coverId.ValueKind == JsonValueKind.Number)
                            {
                                imageUrl = $"https://covers.openlibrary.org/b/id/{coverId.GetInt64()}-M.jpg";
                            }

                            books.Add(new Book
                            {
                                Id = idCounter++,
                                Title = title,
                                Description = "Bu kitab haqqında təsvir və oxucu rəyləri mövcuddur.",
                                CoverImageUrl = imageUrl,
                                Author = new Author { Id = idCounter, Name = authorName },
                                Genre = new Genre { Id = 1, Name = "Dünya Ədəbiyyatı" }
                            });
                        }
                    }
                }
            }
            catch
            {
                // Hər hansı API xətası və ya şəbəkə ləngiməsi olduqda heç bir exception vermir
            }

            // Əgər API-dən kitab gəlməzsə, dərhal işləyən standart kitablarımızı veririk
            return books.Count > 0 ? books : GetDefaultBooks();
        }

        public List<Book> GetDefaultBooks()
        {
            return new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "Əli və Nino",
                    Description = "Şərq və Qərb mədəniyyətlərinin kəsişməsində cərəyan edən məhəbbət hekayəsi.",
                    CoverImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=400",
                    Author = new Author { Id = 1, Name = "Qurban Səid" },
                    Genre = new Genre { Id = 1, Name = "Bədii" }
                },
                new Book
                {
                    Id = 2,
                    Title = "1984",
                    Description = "Totalitar rejimin insan psixologiyası üzərindəki nəzarətini təsvir edən antiutopik roman.",
                    CoverImageUrl = "https://images.unsplash.com/photo-1543002588-bfa74002ed7e?w=400",
                    Author = new Author { Id = 2, Name = "George Orwell" },
                    Genre = new Genre { Id = 2, Name = "Antiutopiya" }
                },
                new Book
                {
                    Id = 3,
                    Title = "Xosrov və Şirin",
                    Description = "Nizami Gəncəvinin ölməz əsəri.",
                    CoverImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?w=400",
                    Author = new Author { Id = 3, Name = "Nizami Gəncəvi" },
                    Genre = new Genre { Id = 3, Name = "Klassika" }
                }
            };
        }
    }
}