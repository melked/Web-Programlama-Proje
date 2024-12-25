using Microsoft.AspNetCore.Mvc;
using KuaforUygulamasi.Models;

namespace KuaforUygulamasi.Controllers
{
    public class CalisanController : Controller
    {
        // Geçici liste (veritabanı yerine)
        private static List<Calisan> calisanlar = new List<Calisan>();

        public IActionResult Index()
        {
            return View(calisanlar);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Calisan calisan)
        {
            if (ModelState.IsValid)
            {
                calisan.ID = calisanlar.Count + 1;
                calisanlar.Add(calisan);
                return RedirectToAction("Index");
            }
            return View(calisan);
        }
    }
}
