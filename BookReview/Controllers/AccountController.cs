using BookReview.Data;
using BookReview.Models;
using BookReview.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookReview.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // 1. QEYDİYYAT (GET)
        public IActionResult Register() => View();

        // 1. QEYDİYYAT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.AnyAsync(u => u.Email == model.Email);
                if (existingUser)
                {
                    ModelState.AddModelError("Email", "Bu email artıq qeydiyyatdan keçib.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = model.Password, // İdeal halda hashing olmalıdır
                    Role = "User"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await SignInUser(user);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // 2. GİRİŞ (GET)
        public IActionResult Login() => View();

        // 2. GİRİŞ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Email və ya şifrə yanlışdır.");
                    return View(model);
                }

                await SignInUser(user);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // 3. ÇIXIŞ (Logout)
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // İstifadəçi Sesiyasını (Cookie) Yaradan Köməkçi Metod
        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
