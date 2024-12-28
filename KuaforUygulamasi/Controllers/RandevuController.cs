using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KuaforUygulamasi.Models;
using KuaforUygulamasi.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Data;

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

        // Randevu Listeleme
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Oturum açmış bir kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // Admin kullanıcıları sadece randevuların listesini görebilir
                var randevular = await _context.Randevular
                    .Include(r => r.Kullanici)
                    .Include(r => r.Calisan)
                    .Include(r => r.Islem)
                    .ToListAsync();
                return View(randevular);
            }
            else
            {
                // Normal kullanıcı yalnızca kendi randevularını görebilir
                var randevular = await _context.Randevular
                    .Where(r => r.KullaniciId == user.Id)
                    .Include(r => r.Calisan)
                    .Include(r => r.Islem)
                    .ToListAsync();
                return View(randevular);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (randevu == null)
                return NotFound();

            // Admin kullanıcıları randevuyu iptal edebilir
            var user = await _userManager.GetUserAsync(User);
            if (randevu.KullaniciId != user.Id && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Bu randevuyu iptal edemezsiniz.";
                return RedirectToAction("Index");
            }

            return View(randevu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Oturum açmış bir kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (randevu == null)
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
                return RedirectToAction("Index", "Randevu");
            }

            // Silme işlemi sadece kullanıcının kendi randevusuna veya admin'e yapılabilir
            if (randevu.KullaniciId != user.Id && !User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "Bu randevuyu iptal edemezsiniz.";
                return RedirectToAction("Index", "Randevu");
            }

            // Randevu iptal işlemi
            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Randevu başarıyla iptal edildi.";
            return RedirectToAction("Index", "Randevu");
        }



        // Randevu Detaylarını Görüntüleme (Kullanıcı ve Admin)
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Oturum açmış bir kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (randevu == null)
                return NotFound();

            // Kullanıcı, sadece kendi randevularını görebilir
            if (randevu.KullaniciId != user.Id && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["ErrorMessage"] = "Bu randevuya erişim izniniz yok.";
                return RedirectToAction("Index", "Home");
            }

            return View(randevu);
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

            ViewBag.KullaniciId = user.Id;
            ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
            ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
            ViewBag.DoluSaatler = await _context.Randevular
                .Select(r => new { r.CalisanID, r.Saat })
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Randevu randevu)
        {
            if (ModelState.IsValid)
            {
                // Seçilen işlemin süresini al
                var islem = await _context.Islemler.FindAsync(randevu.IslemID);
                if (islem == null)
                {
                    ModelState.AddModelError("", "Seçilen işlem bulunamadı.");
                    return View(randevu);
                }

                // Randevunun bitiş zamanını hesapla
                var randevuBitis = randevu.Saat.AddMinutes(islem.Sure);

                // Çakışma kontrolü
                var cakismaVarMi = await _context.Randevular
                    .AnyAsync(r =>
                        r.CalisanID == randevu.CalisanID &&
                        r.ID != randevu.ID && // Kendi randevusu değilse
                        (
                            (randevu.Saat >= r.Saat && randevu.Saat < r.Saat.AddMinutes(r.Islem.Sure)) ||
                            (randevuBitis > r.Saat && randevuBitis <= r.Saat.AddMinutes(r.Islem.Sure)) ||
                            (randevu.Saat <= r.Saat && randevuBitis >= r.Saat.AddMinutes(r.Islem.Sure))
                        )
                    );

                if (cakismaVarMi)
                {
                    ModelState.AddModelError("Saat", "Seçilen saatte başka bir randevu bulunmaktadır.");
                    // ViewBag verilerini tekrar yükle
                    ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
                    ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
                    ViewBag.DoluSaatler = await _context.Randevular
                        .Where(r => r.Saat.Date == randevu.Saat.Date)
                        .Select(r => new { r.CalisanID, r.Saat, IslemSuresi = r.Islem.Sure })
                        .ToListAsync();
                    return View(randevu);
                }

                // Randevuyu kaydet
                _context.Add(randevu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Eğer ModelState geçerli değilse, gerekli verileri tekrar yükle
            ViewBag.CalisanListesi = await _context.Calisanlar.ToListAsync();
            ViewBag.IslemListesi = await _context.Islemler.ToListAsync();
            ViewBag.DoluSaatler = await _context.Randevular
                .Where(r => r.Saat.Date == randevu.Saat.Date)
                .Select(r => new { r.CalisanID, r.Saat, IslemSuresi = r.Islem.Sure })
                .ToListAsync();
            return View(randevu);
        }
    }
}