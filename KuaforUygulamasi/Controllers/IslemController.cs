﻿using KuaforUygulamasi.Data;
using KuaforUygulamasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuaforUygulamasi.Controllers
{
    public class IslemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IslemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Islem/Index
        public async Task<IActionResult> Index()
        {
            var islemler = await _context.Islemler.ToListAsync();
            return View(islemler);
        }

        // GET: Islem/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Islem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Islem islem)
        {
            if (ModelState.IsValid)
            {
                _context.Islemler.Add(islem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(islem);
        }

        // GET: Islem/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var islem = await _context.Islemler.FindAsync(id);
            if (islem == null)
            {
                return NotFound();
            }

            return View(islem);
        }

        // POST: Islem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Islem islem)
        {
            if (id != islem.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(islem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IslemExists(islem.ID))
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

            return View(islem);
        }

        // GET: Islem/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var islem = await _context.Islemler
                .FirstOrDefaultAsync(m => m.ID == id);

            if (islem == null)
            {
                return NotFound();
            }

            return View(islem);
        }

        // POST: Islem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var islem = await _context.Islemler.FindAsync(id);
            if (islem != null)
            {
                _context.Islemler.Remove(islem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IslemExists(int id)
        {
            return _context.Islemler.Any(e => e.ID == id);
        }
    }
}