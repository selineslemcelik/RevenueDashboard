using System.ComponentModel.DataAnnotations;

namespace RevenueDashboard.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    public string Password { get; set; } = string.Empty;
}