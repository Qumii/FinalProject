using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace BookReview.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // "Admin" və ya "User"

        // Əlaqələr
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<UserShelf> UserShelves { get; set; } = new List<UserShelf>();
    }
}
