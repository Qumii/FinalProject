using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookPlatform.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad tələb olunur")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Müəllif tələb olunur")]
        [StringLength(150)]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Janr tələb olunur")]
        [StringLength(60)]
        public string Genre { get; set; } = string.Empty;

        [Range(1000, 2100)]
        public int Year { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        
        public string ContentPreview { get; set; } = string.Empty;

        public string CoverImageUrl { get; set; } = string.Empty;

        public int ReadCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<Review> Reviews { get; set; } = new();

        public List<ShelfItem> ShelfItems { get; set; } = new();

        [NotMapped]
        public double AverageRating => Reviews.Count > 0 ? Reviews.Average(r => r.Rating) : 0;

        [NotMapped]
        public int ReviewCount => Reviews.Count;
    }
}
