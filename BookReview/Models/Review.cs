namespace BookReview.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; } // 1 - 5 arası
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Keys
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }
}




//namespace BookReview.Models
//{
//    public class Review
//    {
//        public int Id { get; set; }
//        public int BookId { get; set; }
//        public string UserName { get; set; } = "Anonim Oxucu";
//        public string Comment { get; set; } = string.Empty;
//        public int Rating { get; set; } = 5; // 1-5 arası ulduz
//        public DateTime CreatedAt { get; set; } = DateTime.Now;
//    }
//}
