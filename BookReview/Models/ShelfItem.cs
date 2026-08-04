namespace BookReview.Models
{
    public class ShelfItem
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public string ShelfType { get; set; } = "ReadLater"; // ReadLater, Reading, Completed
        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}
