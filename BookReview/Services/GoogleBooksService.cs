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
        }

        public async Task<List<Book>> SearchBooksAsync(string query = "Azerbaijan")
        {
            var books = new List<Book>();
            var url = $"https://www.googleapis.com/books/v1/volumes?q={Uri.EscapeDataString(query)}&maxResults=12";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return books;

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);

            if (!doc.RootElement.TryGetProperty("items", out var items)) return books;

            int idCounter = 1;
            foreach (var item in items.EnumerateArray())
            {
                var volumeInfo = item.GetProperty("volumeInfo");

                string title = volumeInfo.TryGetProperty("title", out var t) ? t.GetString()! : "Adsız Kitab";
                string description = volumeInfo.TryGetProperty("description", out var d) ? d.GetString()! : "Məzmun haqqında məlumat yoxdur.";

                string authorName = "Naməlum Müəllif";
                if (volumeInfo.TryGetProperty("authors", out var authors) && authors.EnumerateArray().Any())
                {
                    authorName = authors.EnumerateArray().First().GetString()!;
                }

                string imageUrl = "https://via.placeholder.com/300x400?text=No+Cover";
                if (volumeInfo.TryGetProperty("imageLinks", out var imgLinks) && imgLinks.TryGetProperty("thumbnail", out var thumb))
                {
                    imageUrl = thumb.GetString()!.Replace("http://", "https://");
                }

                books.Add(new Book
                {
                    Id = idCounter++,
                    Title = title,
                    Description = description,
                    CoverImageUrl = imageUrl,
                    Author = new Author { Name = authorName },
                    Genre = new Genre { Name = "Google Books" }
                });
            }

            return books;
        }
    }
}