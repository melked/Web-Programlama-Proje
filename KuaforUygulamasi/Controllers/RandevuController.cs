using KuaforUygulamasi.Data;
using KuaforUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class RandevuController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RandevuController> _logger;

    public RandevuController(ApplicationDbContext context, ILogger<RandevuController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Create()
    {
        try
        {
            var viewModel = new RandevuViewModel()
            {
                IslemListesi = _context.Islemler.ToList(),
                CalisanListesi = _context.Calisanlar.ToList(),
                KullaniciListesi = _context.Users.ToList()
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Randevu oluşturma sayfası yüklenirken hata: {ex.Message}");
            TempData["ErrorMessage"] = "Sayfa yüklenirken bir hata oluştu.";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(RandevuViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Geçersiz model durumu: " + string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));

                model.IslemListesi = await _context.Islemler.ToListAsync();
                model.CalisanListesi = await _context.Calisanlar.ToListAsync();
                model.KullaniciListesi = await _context.Users.ToListAsync();
                return View(model);
            }

            // Geçmiş tarih kontrolü
            if (model.Saat < DateTime.Now)
            {
                ModelState.AddModelError("Saat", "Geçmiş bir tarih seçilemez.");
                model.IslemListesi = await _context.Islemler.ToListAsync();
                model.CalisanListesi = await _context.Calisanlar.ToListAsync();
                model.KullaniciListesi = await _context.Users.ToListAsync();
                return View(model);
            }

            // Yeni Randevu nesnesi oluşturma
            var randevu = new Randevu
            {
                KullaniciID = model.KullaniciID,
                CalisanID = model.CalisanID,
                IslemID = model.IslemID,
                Saat = model.Saat,
                Durum = "Beklemede"
            };

            // Çalışanın o saatlerde başka bir randevusu var mı?
            bool isOverlapping = await _context.Randevular.AnyAsync(r =>
                r.CalisanID == randevu.CalisanID &&
                r.Saat <= randevu.Saat.AddMinutes(30) &&
                r.Saat >= randevu.Saat.AddMinutes(-30));

            if (isOverlapping)
            {
                ModelState.AddModelError("", "Seçilen saat ve çalışan için randevu zaten mevcut.");
                model.IslemListesi = await _context.Islemler.ToListAsync();
                model.CalisanListesi = await _context.Calisanlar.ToListAsync();
                model.KullaniciListesi = await _context.Users.ToListAsync();
                return View(model);
            }

            // Veritabanına ekleme
            try
            {
                _context.Randevular.Add(randevu);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Randevu ID {randevu.ID} başarıyla oluşturuldu.");
                TempData["SuccessMessage"] = "Randevu başarıyla oluşturuldu.";
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Randevu kaydedilirken veritabanı hatası: {ex.Message}");
                ModelState.AddModelError("", "Randevu kaydedilirken bir hata oluştu.");
                model.IslemListesi = await _context.Islemler.ToListAsync();
                model.CalisanListesi = await _context.Calisanlar.ToListAsync();
                model.KullaniciListesi = await _context.Users.ToListAsync();
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Randevu oluşturulurken beklenmeyen hata: {ex.Message}");
            TempData["ErrorMessage"] = "Randevu oluşturulurken bir hata oluştu.";
            return RedirectToAction("Index", "Home");
        }
    }
}
