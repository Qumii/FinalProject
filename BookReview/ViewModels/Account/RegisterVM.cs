using System.ComponentModel.DataAnnotations;

namespace BookReview.ViewModels.Account
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "İstifadəçi adı mütləqdir.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email mütləqdir.")]
        [EmailAddress(ErrorMessage = "Düzgün email daxil edin.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə mütləqdir.")]
        [MinLength(6, ErrorMessage = "Şifrə ən azı 6 simvol olmalıdır.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə təkrarı mütləqdir.")]
        [Compare("Password", ErrorMessage = "Şifrələr üst-üstə düşmür.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
