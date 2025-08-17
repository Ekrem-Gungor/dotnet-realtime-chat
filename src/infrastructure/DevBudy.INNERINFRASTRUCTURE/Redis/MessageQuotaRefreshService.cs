using DevBudy.APPLICATION.Services.RedisServices;
using DevBudy.DOMAIN.CachingModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.INNERINFRASTRUCTURE.Redis
{
    public class MessageQuotaRefreshService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageQuotaRefreshService> _logger;
        public MessageQuotaRefreshService(IServiceProvider serviceProvider, ILogger<MessageQuotaRefreshService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{this.GetType().Name} Started...");

            while (!stoppingToken.IsCancellationRequested)
            {
                List<(string Key, TimeSpan Ttl, MessageQuota Quota)> quotaTtls = new();
                using var scope = _serviceProvider.CreateScope();
                IMessageQuotaService quotaService = scope.ServiceProvider.GetRequiredService<IMessageQuotaService>();

                IEnumerable<string> allKeys = await quotaService.GetAllQuotaKeysAsync();
                foreach (string key in allKeys)
                {
                    MessageQuota quota = await quotaService.GetQuotaAsync(key);

                    if (quota == null)
                        continue;

                    TimeSpan? ttl = await quotaService.GetKeyTtlAsync(key);
                    if (ttl.HasValue)
                    {
                        quotaTtls.Add((key, ttl.Value, quota));
                    }
                }

                (string Key, TimeSpan Ttl, MessageQuota Quota) candidate = quotaTtls.OrderBy(x => x.Ttl).FirstOrDefault();
                if (candidate != default && candidate.Ttl <= TimeSpan.FromSeconds(10))
                {

                    await quotaService.SetQuotaAsync(candidate.Key, new MessageQuota
                    {
                        ReaminingMessage = 30,
                        ExpireAt = candidate.Quota.ExpireAt
                    });
                    _logger.LogInformation($"Quota refreshed for {candidate.Key}");
                }

                // TTL’e göre dinamik bekleme
                TimeSpan minTtl = quotaTtls.Any() ? quotaTtls.Min(x => x.Ttl) : TimeSpan.FromMinutes(1);
                _logger.LogInformation($"Minimum TTL : {minTtl}");

                TimeSpan delay = minTtl > TimeSpan.FromSeconds(5) ? minTtl - TimeSpan.FromSeconds(5) : minTtl;
                if (delay < TimeSpan.Zero) delay = TimeSpan.FromSeconds(10);

                _logger.LogInformation($"Delay : {delay}");
                await Task.Delay(delay, stoppingToken);
            }
            _logger.LogInformation($"{this.GetType().Name} Stopped!");
        }
    }
}
