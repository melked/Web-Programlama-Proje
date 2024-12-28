using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KuaforUygulamasi.Models;
using KuaforUygulamasi.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KuaforUygulamasi.Controllers
{
    public class RandevuController : Controller
    {
        private readonly UserManager<Kullanici> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RandevuController> _logger;

        public RandevuController(
            UserManager<Kullanici> userManager,
            ApplicationDbContext context,
            ILogger<RandevuController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Oturum açmış bir kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["ErrorMessage"] = "Admin kullanıcılar randevu oluşturamaz.";
                return RedirectToAction("Index", "Home");
            }

            // Giriş yapan kullanıcı varsayılan olarak seçiliyor
            ViewBag.KullaniciId = user.Id;

            // Çalışan ve işlem listeleri dolduruluyor
            ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
            ViewBag.IslemListesi = await _context.Islemler.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Randevu randevu)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Oturum açmış bir kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["ErrorMessage"] = "Admin kullanıcılar randevu oluşturamaz.";
                return RedirectToAction("Index", "Home");
            }

            // Kullanıcı kontrolü (Admin dışında bir kullanıcı)
            randevu.KullaniciId = user.Id;

            if (!ModelState.IsValid)
            {
                ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
                ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
                return View(randevu);
            }

            randevu.Durum = "Beklemede";
            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla oluşturuldu.";
            return RedirectToAction("Index", "Home");
        }

        // Randevu düzenleme sayfası
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["ErrorMessage"] = "Admin kullanıcılar randevu düzenleyemez.";
                return RedirectToAction("Index", "Home");
            }

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (randevu == null)
                return NotFound();

            ViewBag.KullaniciListesi = await _userManager.Users.ToListAsync();
            ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
            ViewBag.IslemListesi = await _context.Islemler.ToListAsync();

            return View(randevu);
        }

        // Randevu düzenleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Randevu randevu)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["ErrorMessage"] = "Admin kullanıcılar randevu düzenleyemez.";
                return RedirectToAction("Index", "Home");
            }

            if (id != randevu.ID)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.KullaniciListesi = await _userManager.Users.ToListAsync();
                ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
                ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
                return View(randevu);
            }

            try
            {
                randevu.Saat = randevu.Saat.DateTime.ToUniversalTime();
                _context.Update(randevu);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Randevu başarıyla güncellendi.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu güncellenirken hata oluştu.");
                ModelState.AddModelError("", "Randevu güncellenirken bir hata oluştu.");
                ViewBag.KullaniciListesi = await _userManager.Users.ToListAsync();
                ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
                ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
                return View(randevu);
            }
        }
    }
}
