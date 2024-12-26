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

    // GET: Randevu/CreateRandevu
    [HttpGet]
    public IActionResult CreateRandevu()
    {
        // Initialize the view model and provide lists for select options
        return View(new RandevuViewModel()
        {
            IslemListesi = _context.Islemler.ToList(),
            CalisanListesi = _context.Calisanlar.ToList(),
            KullaniciListesi = _context.Users.ToList()
        });
    }

    // POST: Randevu/CreateRandevu
    [HttpPost]
    public async Task<IActionResult> CreateRandevu(RandevuViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Eğer model geçerli değilse, kullanıcıyı aynı sayfada tutuyoruz ve hataları gösteriyoruz
            model.IslemListesi = _context.Islemler.ToList();
            model.CalisanListesi = _context.Calisanlar.ToList();
            model.KullaniciListesi = _context.Users.ToList();
            return View(model);
        }

        // Randevu nesnesini oluşturuyoruz
        var randevu = new Randevu
        {
            KullaniciID = model.KullaniciID,
            CalisanID = model.CalisanID,
            IslemID = model.IslemID,
            Saat = model.Saat,
            Durum = "Beklemede"  // Varsayılan durum
        };

        // Çakışma kontrolü (aynı çalışan ve saat için randevu varsa)
        bool isOverlapping = await _context.Randevular.AnyAsync(r =>
            r.CalisanID == randevu.CalisanID && r.Saat == randevu.Saat);

        if (isOverlapping)
        {
            ModelState.AddModelError("", "Seçilen saat ve çalışan için randevu zaten mevcut.");
            model.IslemListesi = _context.Islemler.ToList();
            model.CalisanListesi = _context.Calisanlar.ToList();
            model.KullaniciListesi = _context.Users.ToList();
            return View(model);
        }

        // Randevuyu ekliyoruz
        _context.Randevular.Add(randevu);
        await _context.SaveChangesAsync();

        // Başarıyla ekledikten sonra yönlendirme yapıyoruz
        _logger.LogInformation($"Randevu ID {randevu.ID} başarıyla oluşturuldu.");
        return RedirectToAction("Index", "Home");
    }
}