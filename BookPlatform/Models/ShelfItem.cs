using System.ComponentModel.DataAnnotations.Schema;

namespace BookPlatform.Models
{
    
    public class ShelfItem
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
