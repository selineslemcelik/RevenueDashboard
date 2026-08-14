using System.ComponentModel.DataAnnotations;

namespace RevenueDashboard.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(4, ErrorMessage = "Şifre en az 4 karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;
}