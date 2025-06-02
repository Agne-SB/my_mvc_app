using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using MyMvcApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

public class AccountController : Controller
{
    private readonly HttpClient _authHttp;

    public AccountController(IHttpClientFactory factory)
    {
        _authHttp = factory.CreateClient("AuthApi");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await _authHttp.PostAsJsonAsync("api/auth/login", model);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            TempData["Token"] = result.AccessToken;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Login");
    }
}
