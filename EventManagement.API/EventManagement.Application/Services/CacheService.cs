using System;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Helpers;
using EventManagement.Application.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace EventManagement.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILoggerManager<CacheService> _loggerManager;
        private readonly RedisConnection _cacheSettings;

        public CacheService(IDistributedCache distributedCache, IOptions<RedisConnection> cacheSettings, ILoggerManager<CacheService> loggerManager)
        {
            this._distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
            this._cacheSettings = cacheSettings?.Value ?? throw new ArgumentNullException(nameof(cacheSettings));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var resultCache = await this._distributedCache.GetAsync(key);
            return resultCache?.Length > 0 ? resultCache.Deserialize<T>() : default;
        }

        public async Task SetAsync<T>(string key, T cacheData, TimeSpan? time = null, TimeSpan? slidingTime = null)
        {
            if (key == null || cacheData == null)
            {
                throw new ArgumentNullException();
            }

            var cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(time ?? TimeSpan.FromHours(this._cacheSettings.DefaultTimeInHours))
                .SetSlidingExpiration(slidingTime ?? TimeSpan.FromHours(1));
            await this._distributedCache.SetAsync(key, cacheData.Serailize(), cacheEntryOptions);
        }

        public async Task RemoveByKey(string key)
        {
            await this._distributedCache.RemoveAsync(key);
            this._loggerManager.LogInformation(new
            {
                Time = DateTime.Now,
                Message = "CACHE CLEARED",
                KEY = key
            });
        }
    }
}