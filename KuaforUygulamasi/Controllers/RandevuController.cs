using Microsoft.AspNetCore.Mvc;
using KuaforUygulamasi.Models;

namespace KuaforUygulamasi.Controllers
{
    public class RandevuController : Controller
    {
        // Geçici bir liste ile veri tutma
        private static List<Randevu> randevular = new List<Randevu>();

        // Randevuları listeleyen Action
        public IActionResult Index()
        {
            return View(randevular);
        }

        // Yeni randevu ekleme formu
        public IActionResult Create()
        {
            return View();
        }

        // Yeni randevuyu ekleyen Action (POST)
        [HttpPost]
        public IActionResult Create(Randevu randevu)
        {
            if (ModelState.IsValid)
            {
                randevu.Id = randevular.Count + 1;
                randevular.Add(randevu);
                return RedirectToAction("Index");
            }
            return View(randevu);
        }

        // Randevuyu silen Action
        public IActionResult Delete(int id)
        {
            var randevu = randevular.FirstOrDefault(r => r.Id == id);
            if (randevu != null)
            {
                randevular.Remove(randevu);
            }
            return RedirectToAction("Index");
        }
    }
}
