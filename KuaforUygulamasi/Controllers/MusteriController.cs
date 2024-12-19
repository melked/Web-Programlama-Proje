using System;
using Microsoft.AspNetCore.Mvc;
using KuaforUygulamasi.Models;

namespace KuaforUygulamasi.Controllers
{
    public class MusteriController : Controller
    {
        // Geçici bir liste ile müşteri verisi tutma
        private static List<Musteri> musteriler = new List<Musteri>();
        private static List<Randevu> randevular = new List<Randevu>(); // Randevu listesi referansı

        // Müşterileri listeleyen Action
        public IActionResult Index()
        {
            return View(musteriler);
        }

        // Yeni müşteri ekleme formu
        public IActionResult Create()
        {
            return View();
        }

        // Yeni müşteri ekleyen Action (POST)
        [HttpPost]
        public IActionResult Create(Musteri musteri)
        {
            if (ModelState.IsValid)
            {
                musteri.Id = musteriler.Count + 1;
                musteriler.Add(musteri);
                return RedirectToAction("Index");
            }
            return View(musteri);
        }

        // Müşteri düzenleme formu
        public IActionResult Edit(int id)
        {
            var musteri = musteriler.FirstOrDefault(m => m.Id == id);
            if (musteri == null)
            {
                return NotFound();
            }
            return View(musteri);
        }

        // Müşteri düzenleyen Action (POST)
        [HttpPost]
        public IActionResult Edit(Musteri musteri)
        {
            var mevcutMusteri = musteriler.FirstOrDefault(m => m.Id == musteri.Id);
            if (mevcutMusteri != null && ModelState.IsValid)
            {
                mevcutMusteri.Ad = musteri.Ad;
                mevcutMusteri.Telefon = musteri.Telefon;
                mevcutMusteri.Email = musteri.Email;
                return RedirectToAction("Index");
            }
            return View(musteri);
        }

        // Müşteri silen Action
        public IActionResult Delete(int id)
        {
            var musteri = musteriler.FirstOrDefault(m => m.Id == id);
            if (musteri != null)
            {
                musteriler.Remove(musteri);
            }
            return RedirectToAction("Index");
        }

        // Müşterinin geçmiş randevularını görüntüleme
        public IActionResult Randevular(int id)
        {
            var musteriRandevulari = randevular.Where(r => r.MusteriId == id).ToList();
            ViewBag.MusteriAdi = musteriler.FirstOrDefault(m => m.Id == id)?.Ad;
            return View(musteriRandevulari);
        }
    }
}
