using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KuaforUygulamasi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KuaforUygulamasi.Data;

namespace KuaforUygulamasi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RandevuController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Randevu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Randevu>>> GetAllRandevular()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .ToListAsync();

            return Ok(randevular);
        }

        // GET: api/Randevu/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Randevu>> GetRandevuById(int id)
        {
            var randevu = await _context.Randevular
                .Include(r => r.Kullanici)
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (randevu == null)
            {
                return NotFound($"Randevu ID {id} bulunamadı.");
            }

            return Ok(randevu);
        }

        // POST: api/Randevu
        [HttpPost]
        public async Task<ActionResult> CreateRandevu([FromBody] Randevu randevu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRandevuById), new { id = randevu.ID }, randevu);
        }

        // PUT: api/Randevu/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRandevu(int id, [FromBody] Randevu randevu)
        {
            if (id != randevu.ID)
            {
                return BadRequest("Randevu ID uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(randevu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Randevular.Any(r => r.ID == id))
                {
                    return NotFound($"Randevu ID {id} bulunamadı.");
                }

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Randevu/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRandevu(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);

            if (randevu == null)
            {
                return NotFound($"Randevu ID {id} bulunamadı.");
            }

            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
