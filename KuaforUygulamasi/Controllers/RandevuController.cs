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
                model.IslemListesi = await _context.Islemler.ToListAsync();
                model.CalisanListesi = await _context.Calisanlar.ToListAsync();
                model.KullaniciListesi = await _context.Users.ToListAsync();
                return View(model);
            }

            // Çalışanın bilgilerini al
            var calisan = await _context.Calisanlar.FindAsync(model.CalisanID);
            if (calisan == null)
            {
                ModelState.AddModelError("", "Seçilen çalışan bulunamadı.");
                model.IslemListesi = await _context.Islemler.ToListAsync();
                model.CalisanListesi = await _context.Calisanlar.ToListAsync();
                model.KullaniciListesi = await _context.Users.ToListAsync();
                return View(model);
            }

            // İşlemin bilgilerini al
            var islem = await _context.Islemler.FindAsync(model.IslemID);
            if (islem == null)
            {
                ModelState.AddModelError("", "Seçilen işlem bulunamadı.");
                model.IslemListesi = await _context.Islemler.ToListAsync();
                model.CalisanListesi = await _context.Calisanlar.ToListAsync();
                model.KullaniciListesi = await _context.Users.ToListAsync();
                return View(model);
            }

            // Çalışanın müsaitlik saatleriyle randevu saatini kontrol et
            DateTime randevuBitisSaati = model.Saat.AddMinutes(islem.Sure);
            if (model.Saat.TimeOfDay < calisan.MusaitlikBaslangic || randevuBitisSaati.TimeOfDay > calisan.MusaitlikBitis)
            {
                ModelState.AddModelError("Saat", $"Çalışan {calisan.Ad} sadece {calisan.MusaitlikBaslangic} - {calisan.MusaitlikBitis} saatleri arasında müsait.");
                model.IslemListesi = await _context.Islemler.ToListAsync();
                model.CalisanListesi = await _context.Calisanlar.ToListAsync();
                model.KullaniciListesi = await _context.Users.ToListAsync();
                return View(model);
            }

            // Çalışanın aynı saatlerde başka bir randevusu var mı?
            bool isOverlapping = await _context.Randevular.AnyAsync(r =>
                r.CalisanID == model.CalisanID &&
                (
                    (model.Saat >= r.Saat && model.Saat < r.Saat.AddMinutes(islem.Sure)) || // Yeni randevu, mevcut randevuyla çakışıyor
                    (randevuBitisSaati > r.Saat && randevuBitisSaati <= r.Saat.AddMinutes(islem.Sure)) // Mevcut randevu, yeni randevunun içine çakışıyor
                ));

            if (isOverlapping)
            {
                ModelState.AddModelError("", "Seçilen saat ve çalışan için randevu zaten mevcut.");
                model.IslemListesi = await _context.Islemler.ToListAsync();
                model.CalisanListesi = await _context.Calisanlar.ToListAsync();
                model.KullaniciListesi = await _context.Users.ToListAsync();
                return View(model);
            }

            // Randevu ekle
            var randevu = new Randevu
            {
                KullaniciID = model.KullaniciID,
                CalisanID = model.CalisanID,
                IslemID = model.IslemID,
                Saat = model.Saat,
                Durum = "Beklemede"
            };

            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Randevu başarıyla oluşturuldu.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            // Hata loglama ve kullanıcıya bilgi verme
            _logger.LogError($"Randevu oluşturulurken hata: {ex.Message}");
            TempData["ErrorMessage"] = "Randevu oluşturulurken bir hata oluştu.";
            return RedirectToAction("Index", "Home");
        }
    }
}
