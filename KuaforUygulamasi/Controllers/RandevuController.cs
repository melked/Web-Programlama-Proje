// POST: Randevu/CreateRandevu
using KuaforUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    // Redirect to Home controller after successful appointment creation
    return RedirectToAction("Index", "Home");
}
