using System.ComponentModel.DataAnnotations;

namespace BookPlatform.ViewModels.Account
{
    public class LoginVM
    {
        [Required(ErrorMessage = "E-poçt tələb olunur")]
        [EmailAddress]
        [Display(Name = "E-poçt")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə tələb olunur")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifrə")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Məni yadda saxla")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
