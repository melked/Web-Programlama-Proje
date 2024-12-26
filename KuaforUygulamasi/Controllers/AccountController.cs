using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KuaforUygulamasi.Models;
using System.Threading.Tasks;

namespace KuaforUygulamasi.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Kullanici> _userManager;
        private readonly SignInManager<Kullanici> _signInManager;

        public AccountController(UserManager<Kullanici> userManager, SignInManager<Kullanici> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string sifre)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sifre))
            {
                ModelState.AddModelError("", "E-posta ve şifre alanları doldurulmalıdır.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, sifre, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Geçersiz e-posta veya şifre.");
            return View();
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string sifre, string ad, string soyad)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sifre) || string.IsNullOrWhiteSpace(ad) || string.IsNullOrWhiteSpace(soyad))
            {
                ModelState.AddModelError("", "Tüm alanlar doldurulmalıdır.");
                return View();
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Bu e-posta adresi zaten kayıtlı.");
                return View();
            }

            var user = new Kullanici
            {
                UserName = email,
                Email = email,
                Ad = ad,
                Soyad = soyad,
                Rol = "User" // Varsayılan rol
            };

            var result = await _userManager.CreateAsync(user, sifre); // Şifre otomatik olarak hashlenir.
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }

        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
