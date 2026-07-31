namespace BookReview.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Əlaqələr (Bir janrda çoxlu kitab ola bilər)
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
