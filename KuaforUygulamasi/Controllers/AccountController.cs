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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string sifre)
        {
            var result = await _signInManager.PasswordSignInAsync(email, sifre, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "E-posta veya şifre hatalı.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Register(string email, string sifre, string ad, string soyad)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre) || string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad))
            {
                ModelState.AddModelError("", "Tüm alanlar doldurulmalıdır.");
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

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
