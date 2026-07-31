namespace BookReview.Models
{

    public enum ShelfStatus
    {
        WantToRead, // Oxuyacaqlarım
        Reading,    // Hal-hazırda oxuduqlarım
        Read        // Oxuduqlarım
    }
    public class UserShelf
    {

        public int Id { get; set; }
        public ShelfStatus Status { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}
