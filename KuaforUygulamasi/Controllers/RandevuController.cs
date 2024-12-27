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
            // 1. Kullanıcı, çalışan ve işlem kontrolü
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

            // 3. Çalışanın müsaitlik saatleri kontrolü
            if (calisan != null)
            {
                var randevuSaati = randevu.Saat.TimeOfDay;
                if (randevuSaati < calisan.MusaitlikBaslangic || randevuSaati > calisan.MusaitlikBitis)
                {
                    ModelState.AddModelError("Saat", $"Çalışan {calisan.Ad} sadece {calisan.MusaitlikBaslangic} - {calisan.MusaitlikBitis} saatleri arasında müsait.");
                }
            }

            // 4. Aynı saatte başka bir randevu kontrolü
            var existingAppointment = await _context.Randevular
                .AnyAsync(r => r.CalisanID == randevu.CalisanID
                            && r.Saat == randevu.Saat
                            && r.Durum != "İptal");

            if (existingAppointment)
            {
                ModelState.AddModelError("Saat", "Bu saat dilimi dolu.");
            }

            // 5. Model doğrulama hataları varsa işlemi durdur
            if (!ModelState.IsValid)
            {
                ViewBag.KullaniciListesi = await _userManager.Users.ToListAsync();
                ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
                ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
                return View(randevu);
            }

            try
            {
                // 6. Tarih ve saat UTC olarak ayarlanır
                randevu.Saat = randevu.Saat.DateTime.ToUniversalTime();
                randevu.Durum = "Beklemede";

                _context.Randevular.Add(randevu);
                await _context.SaveChangesAsync();

                // Başarılı işlem sonrası yönlendirme
                TempData["SuccessMessage"] = "Randevu başarıyla oluşturuldu.";
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

        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .ToListAsync();

            return View(randevular);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var randevu = await _context.Randevular.FindAsync(id);
                if (randevu == null)
                {
                    return NotFound();
                }

                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Randevu başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu silinirken hata oluştu. Randevu ID: {RandevuId}", id);
                TempData["ErrorMessage"] = "Randevu silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (randevu == null)
            {
                return NotFound();
            }

            ViewBag.KullaniciListesi = await _userManager.Users.ToListAsync();
            ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
            ViewBag.IslemListesi = await _context.Islemler.ToListAsync();

            return View(randevu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Randevu randevu)
        {
            if (id != randevu.ID)
            {
                return NotFound();
            }

            // Validation checks similar to Create action
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

            // Check if the appointment time is in the past
            if (randevu.Saat < DateTime.UtcNow)
            {
                ModelState.AddModelError("Saat", "Geçmiş bir tarih seçilemez.");
            }

            // Check employee availability
            if (calisan != null)
            {
                var randevuSaati = randevu.Saat.TimeOfDay;
                if (randevuSaati < calisan.MusaitlikBaslangic || randevuSaati > calisan.MusaitlikBitis)
                {
                    ModelState.AddModelError("Saat", $"Çalışan {calisan.Ad} sadece {calisan.MusaitlikBaslangic} - {calisan.MusaitlikBitis} saatleri arasında müsait.");
                }
            }

            // Check for conflicting appointments
            var existingAppointment = await _context.Randevular
                .AnyAsync(r => r.CalisanID == randevu.CalisanID
                            && r.Saat == randevu.Saat
                            && r.ID != randevu.ID  // Exclude current appointment
                            && r.Durum != "İptal");

            if (existingAppointment)
            {
                ModelState.AddModelError("Saat", "Bu saat dilimi dolu.");
            }

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
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await RandevuExists(randevu.ID))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Randevu güncellenirken hata oluştu. Randevu ID: {RandevuId}", id);
                    ModelState.AddModelError("", "Randevu güncellenirken bir hata oluştu. Lütfen tekrar deneyiniz.");
                    ViewBag.KullaniciListesi = await _userManager.Users.ToListAsync();
                    ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
                    ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
                    return View(randevu);
                }
            }
        }

        private async Task<bool> RandevuExists(int id)
        {
            return await _context.Randevular.AnyAsync(r => r.ID == id);
        }

    }
}