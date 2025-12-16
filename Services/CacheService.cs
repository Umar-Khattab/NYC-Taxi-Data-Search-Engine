using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Collections;

namespace NycTaxiSearch.Services
{
    public class CacheService<T> : ICacheService<T> where T : class
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        // Change the type of _cacheKeys from HashSet<T> to HashSet<string>
        private static readonly HashSet<string> _cacheKeys = new();

        public CacheService(IMemoryCache cache, ILogger logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task GetAsync(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out T? value))
                {
                    _logger.LogInformation("Cache hit for key: {Key}", key);
                    return Task.FromResult(value);
                }

                _logger.LogInformation("Cache miss for key: {Key}", key);
                return Task.FromResult(default(T));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving from cache for key: {Key}", key);
                return Task.FromResult(default(T));
            }
        }

        public Task SetAsync(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };

                _cache.Set(key, value, cacheOptions);
                _cacheKeys.Add(key);
                _logger.LogInformation("Cached value for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                _cacheKeys.Remove(key);
                _logger.LogInformation("Removed cache for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
            }

            return Task.CompletedTask;
        }

        public Task ClearAsync(string pattern)
        {
            try
            {
                var keysToRemove = _cacheKeys.Where(k => k.Contains(pattern)).ToList();
                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                    _cacheKeys.Remove(key);
                }
                _logger.LogInformation("Cleared {Count} cache entries matching pattern: {Pattern}",
                    keysToRemove.Count, pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cache with pattern: {Pattern}", pattern);
            }

            return Task.CompletedTask;
        }
    }
}