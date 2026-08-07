using System.ComponentModel.DataAnnotations;

namespace BookPlatform.Models.ViewModels
{
    public class BookVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad tələb olunur")]
        [StringLength(200)]
        [Display(Name = "Kitabın adı")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Müəllif tələb olunur")]
        [StringLength(150)]
        [Display(Name = "Müəllif")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Janr tələb olunur")]
        [Display(Name = "Janr")]
        public string Genre { get; set; } = string.Empty;

        [Range(1000, 2100, ErrorMessage = "Düzgün il daxil et")]
        [Display(Name = "Nəşr ili")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Qısa təsvir tələb olunur")]
        [StringLength(1000)]
        [Display(Name = "Qısa təsvir")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Oxu mətni (nümunə)")]
        [StringLength(4000)]
        public string? ContentPreview { get; set; }

        [Display(Name = "Cild şəkli")]
        public IFormFile? CoverImage { get; set; }

        public string? ExistingCoverImageUrl { get; set; }
    }

    public class ReviewVM
    {
        public int BookId { get; set; }

        [Range(1, 5, ErrorMessage = "Ulduz seç")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Rəy mətni boş ola bilməz")]
        [StringLength(1000)]
        public string Text { get; set; } = string.Empty;
    }
}
