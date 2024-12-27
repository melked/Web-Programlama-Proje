using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KuaforUygulamasi.Models;
using KuaforUygulamasi.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

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
            ViewBag.KullaniciListesi = await _userManager.Users.ToListAsync();
            ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
            ViewBag.IslemListesi = await _context.Islemler.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Randevu randevu)
        {
            // 1. Geçerli kullanıcı, çalışan ve işlem kontrolü
            var kullanici = await _userManager.FindByIdAsync(randevu.KullaniciId);
            var calisan = await _context.Calisanlar.FindAsync(randevu.CalisanID);
            var islem = await _context.Islemler.FindAsync(randevu.IslemID);

            if (kullanici == null)
            {
                ModelState.AddModelError("KullaniciId", "Geçersiz müşteri seçimi.");
            }
            if (calisan == null)
            {
                ModelState.AddModelError("CalisanID", "Geçersiz çalışan seçimi.");
            }
            if (islem == null)
            {
                ModelState.AddModelError("IslemID", "Geçersiz işlem seçimi.");
            }

            // 2. Geçmiş bir tarih kontrolü
            if (randevu.Saat < DateTime.UtcNow)
            {
                ModelState.AddModelError("Saat", "Geçmiş bir tarih seçilemez.");
            }

            // 3. Seçilen saatteki doluluk kontrolü
            var existingAppointment = await _context.Randevular
                .AnyAsync(r => r.CalisanID == randevu.CalisanID
                            && r.Saat == randevu.Saat
                            && r.Durum != "İptal");

            if (existingAppointment)
            {
                ModelState.AddModelError("Saat", "Bu saat dilimi dolu.");
            }

            // 4. Model doğrulama hataları varsa işlemi durdur
            if (!ModelState.IsValid)
            {
                ViewBag.KullaniciListesi = await _userManager.Users.ToListAsync();
                ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
                ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
                return View(randevu);
            }

            try
            {
                // 5. Tarih ve saat UTC olarak ayarlanır
                randevu.Saat = randevu.Saat.DateTime.ToUniversalTime(); // `DateTimeOffset`'i `DateTime`'a dönüştürme

                randevu.Durum = "Beklemede";

                _context.Randevular.Add(randevu);
                await _context.SaveChangesAsync();

                // Başarılı işlem sonrası yönlendirme
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Hata loglama ve kullanıcıya hata mesajı gösterme
                _logger.LogError(ex, "Randevu kaydedilirken hata oluştu.");
                ModelState.AddModelError("", "Randevu kaydedilirken bir hata oluştu. Lütfen tekrar deneyiniz.");
                ViewBag.KullaniciListesi = await _userManager.Users.ToListAsync();
                ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
                ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
                return View(randevu);
            }
        }
    }
}
