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
        private readonly IServiceScopeFactory _scopeFactory;
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
                await ProcessRandevularAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);  // 5 dakikada bir kontrol et
            }
        }

        private async Task ProcessRandevularAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await CheckAndUpdateRandevular(context, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Randevu kontrol sürecinde hata oluştu: {ex.Message}");
            }
        }

        private async Task CheckAndUpdateRandevular(ApplicationDbContext context, CancellationToken stoppingToken)
        {
            // Randevuları sadece onaylanmamış ve geçerli bir zaman diliminde olanları getirelim
            var randevular = await context.Randevular
                .Where(r => r.Saat <= DateTime.Now.AddHours(3) && r.Durum != "Onaylı")  // 3 saatten önceki onaysız randevular
                .ToListAsync(stoppingToken);

            if (!randevular.Any())
            {
                _logger.LogInformation("Onaylanacak randevu bulunmadı.");
                return;
            }

            foreach (var randevu in randevular)
            {
                // Randevuları onayla
                randevu.Durum = "Onaylı";
                context.Randevular.Update(randevu);
            }

            // Veritabanını güncelle
            await context.SaveChangesAsync(stoppingToken);
            _logger.LogInformation($"{randevular.Count} randevu başarıyla onaylandı.");
        }
    }
}
