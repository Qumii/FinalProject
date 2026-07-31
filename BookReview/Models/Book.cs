using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace BookReview.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public int PublishedYear { get; set; }

        // Foreign Keys
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        // Əlaqələr
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<UserShelf> UserShelves { get; set; } = new List<UserShelf>();
    }
}
