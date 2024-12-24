using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KuaforUygulamasi.Models;
using KuaforUygulamasi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KuaforUygulamasi.Controllers
{
    [ApiController]
    [Route("[controller]")]

    [Authorize]
    public class RandevuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RandevuController> _logger;

        public RandevuController(ApplicationDbContext context, ILogger<RandevuController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Randevu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Randevu>>> GetAllRandevular(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Tüm randevular getiriliyor.");

            var randevular = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(randevular);
        }

        // GET: api/Randevu/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Randevu>> GetRandevuById(int id)
        {
            _logger.LogInformation($"Randevu ID {id} getiriliyor.");

            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (randevu == null)
            {
                _logger.LogWarning($"Randevu ID {id} bulunamadı.");
                return NotFound(new { Message = $"Randevu ID {id} bulunamadı." });
            }

            return Ok(randevu);
        }

        // POST: api/Randevu
        [HttpPost]
        public async Task<ActionResult> CreateRandevu([FromBody] Randevu randevu)
        {
            _logger.LogInformation("Yeni randevu oluşturuluyor.");

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Geçersiz veri.", Errors = ModelState });
            }

            // Çakışma kontrolü
            bool isOverlapping = await _context.Randevular.AnyAsync(r =>
                r.CalisanID == randevu.CalisanID &&
                r.Saat == randevu.Saat);

            if (isOverlapping)
            {
                return BadRequest(new { Message = "Seçilen saat ve çalışan için randevu zaten mevcut." });
            }

            randevu.Durum = "Beklemede";
            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Randevu ID {randevu.ID} başarıyla oluşturuldu.");

            return CreatedAtAction(nameof(GetRandevuById), new { id = randevu.ID }, randevu);
        }

        // PUT: api/Randevu/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRandevu(int id, [FromBody] Randevu randevu)
        {
            if (id != randevu.ID)
            {
                return BadRequest(new { Message = "Randevu ID uyuşmuyor." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Geçersiz veri.", Errors = ModelState });
            }

            var existingRandevu = await _context.Randevular.FindAsync(id);

            if (existingRandevu == null)
            {
                return NotFound(new { Message = $"Randevu ID {id} bulunamadı." });
            }

            existingRandevu.KullaniciID = randevu.KullaniciID;
            existingRandevu.CalisanID = randevu.CalisanID;
            existingRandevu.IslemID = randevu.IslemID;
            existingRandevu.Saat = randevu.Saat;
            existingRandevu.Durum = randevu.Durum;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Randevu ID {id} başarıyla güncellendi.");
            return NoContent();
        }

        // DELETE: api/Randevu/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteRandevu(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);

            if (randevu == null)
            {
                _logger.LogWarning($"Randevu ID {id} bulunamadı.");
                return NotFound(new { Message = $"Randevu ID {id} bulunamadı." });
            }

            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Randevu ID {id} başarıyla silindi.");
            return NoContent();
        }

        // GET: api/Randevu/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Randevu>>> SearchRandevular(DateTime? startDate, DateTime? endDate, int? calisanId, int? kullaniciId)
        {
            _logger.LogInformation("Randevu arama işlemi başlatıldı.");

            var query = _context.Randevular.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(r => r.Saat >= startDate);

            if (endDate.HasValue)
                query = query.Where(r => r.Saat <= endDate);

            if (calisanId.HasValue)
                query = query.Where(r => r.CalisanID == calisanId);

            if (kullaniciId.HasValue)
                query = query.Where(r => r.KullaniciID == kullaniciId);

            var randevular = await query
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .ToListAsync();

            return Ok(randevular);
        }
    }
}