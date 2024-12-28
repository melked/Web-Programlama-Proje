using KuaforUygulamasi.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KuaforUygulamasi.Services
{
    public class RandevuOnayService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;  // IServiceScopeFactory kullanın
        private readonly ILogger<RandevuOnayService> _logger;

        public RandevuOnayService(IServiceScopeFactory scopeFactory, ILogger<RandevuOnayService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())  // Scoped servisi oluşturun
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // DbContext'i al
                    await CheckAndUpdateRandevular(context); // Veritabanı işlemi
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // 5 dakika aralıklarla çalışacak
            }
        }

        public async Task CheckAndUpdateRandevular(ApplicationDbContext context)
        {
            try
            {
                var randevular = await context.Randevular
                    .Where(r => r.Saat <= DateTime.Now.AddHours(3) && r.Durum != "Onaylı")
                    .ToListAsync();

                foreach (var randevu in randevular)
                {
                    randevu.Durum = "Onaylı";  // Randevu onaylı hale gelir
                    context.Randevular.Update(randevu);
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("Randevular başarıyla onaylandı.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Randevuların onaylanmasında bir hata oluştu: {ex.Message}");
            }
        }
    }


}
