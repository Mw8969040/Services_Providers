using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SmartPlatform.Application.Common.Interfaces;

namespace SmartPlatform.Infrastructure.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly bool _enableCaching;

        public MemoryCacheService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _enableCaching = configuration.GetValue<bool>("EnableCaching", true);
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (!_enableCaching) return Task.FromResult(default(T));

            _memoryCache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            if (!_enableCaching) return Task.CompletedTask;

            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration); // Using absolute expiration as requested in optional
                
            _memoryCache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
