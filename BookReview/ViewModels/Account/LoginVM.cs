using System.ComponentModel.DataAnnotations;

namespace BookReview.ViewModels.Account
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email mütləqdir.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə mütləqdir.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
