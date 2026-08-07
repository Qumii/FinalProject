using System.ComponentModel.DataAnnotations;

namespace BookPlatform.ViewModels.Account
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Ad tələb olunur")]
        [StringLength(100)]
        [Display(Name = "Adın Soyadın")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-poçt tələb olunur")]
        [EmailAddress(ErrorMessage = "Düzgün e-poçt daxil et")]
        [Display(Name = "E-poçt")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə tələb olunur")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifrə ən azı 6 simvol olmalıdır")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifrə")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Şifrəni təkrarla")]
        [Compare("Password", ErrorMessage = "Şifrələr üst-üstə düşmür")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
