using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KuaforUygulamasi.Data;
using KuaforUygulamasi.Models;
using KuaforUygulamasi.ViewModels;

namespace KuaforUygulamasi.Controllers
{
    public class CalisanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalisanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var calisanlar = _context.Calisanlar.ToList();
            return View(calisanlar);
        }

        public IActionResult Create()
        {
            var viewModel = new CalisanViewModel
            {
                MusaitlikBaslangic = new TimeSpan(9, 0, 0),
                MusaitlikBitis = new TimeSpan(18, 0, 0)
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CalisanViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var calisan = new Calisan
                {
                    Ad = viewModel.Ad,
                    Soyad = viewModel.Soyad,
                    UzmanlikAlanlari = viewModel.SelectedUzmanlikAlanlari,
                    MusaitlikBaslangic = viewModel.MusaitlikBaslangic,
                    MusaitlikBitis = viewModel.MusaitlikBitis
                };

                _context.Calisanlar.Add(calisan);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Çalışan başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calisan = await _context.Calisanlar
                .FirstOrDefaultAsync(m => m.ID == id);

            if (calisan == null)
            {
                return NotFound();
            }

            return View(calisan);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calisan = await _context.Calisanlar.FindAsync(id);
            if (calisan == null)
            {
                return NotFound();
            }

            var viewModel = new CalisanViewModel
            {
                ID = calisan.ID,
                Ad = calisan.Ad,
                Soyad = calisan.Soyad,
                SelectedUzmanlikAlanlari = calisan.UzmanlikAlanlari,
                MusaitlikBaslangic = calisan.MusaitlikBaslangic,
                MusaitlikBitis = calisan.MusaitlikBitis
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CalisanViewModel viewModel)
        {
            if (id != viewModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var calisan = await _context.Calisanlar.FindAsync(id);
                    if (calisan == null)
                    {
                        return NotFound();
                    }

                    calisan.Ad = viewModel.Ad;
                    calisan.Soyad = viewModel.Soyad;
                    calisan.UzmanlikAlanlari = viewModel.SelectedUzmanlikAlanlari;
                    calisan.MusaitlikBaslangic = viewModel.MusaitlikBaslangic;
                    calisan.MusaitlikBitis = viewModel.MusaitlikBitis;

                    _context.Update(calisan);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Çalışan başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalisanExists(viewModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calisan = await _context.Calisanlar
                .FirstOrDefaultAsync(m => m.ID == id);
            if (calisan == null)
            {
                return NotFound();
            }

            return View(calisan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calisan = await _context.Calisanlar.FindAsync(id);
            if (calisan != null)
            {
                _context.Calisanlar.Remove(calisan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Çalışan başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CalisanExists(int id)
        {
            return _context.Calisanlar.Any(e => e.ID == id);
        }
    }
}