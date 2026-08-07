using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookPlatform.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Qiymət 1 ilə 5 arasında olmalıdır")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Rəy mətni boş ola bilməz")]
        [StringLength(1000)]
        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
