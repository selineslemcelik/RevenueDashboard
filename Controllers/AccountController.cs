using Microsoft.AspNetCore.Mvc;
using RevenueDashboard.Models.ViewModels;
using RevenueDashboard.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace RevenueDashboard.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var success = await _accountService.RegisterAsync(model.Username, model.Password);
        if (!success)
        {
            ModelState.AddModelError("", "Bu kullanıcı adı zaten alınmış.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
        return RedirectToAction("Login");
    }
    [HttpGet]
public IActionResult Login()
{
    return View();
}

[HttpPost]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    var user = await _accountService.LoginAsync(model.Username, model.Password);
    if (user == null)
    {
        ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
        return View(model);
    }

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var identity = new ClaimsIdentity(claims, "CookieAuth");
    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync("CookieAuth", principal);

    return RedirectToAction("Index", "Dashboard");
}

[HttpPost]
public async Task<IActionResult> Logout()
{
    await HttpContext.SignOutAsync("CookieAuth");
    return RedirectToAction("Login");
}
}