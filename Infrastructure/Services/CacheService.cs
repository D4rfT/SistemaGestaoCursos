using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly List<string> _cacheKeys = new();

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiration = null)
        {
            // Tenta obter do cache primeiro
            if (_cache.TryGetValue(key, out T cachedValue))
            {
                _logger.LogDebug("Cache hit: {Key}", key);
                return cachedValue;
            }

            _logger.LogDebug("Cache miss: {Key}", key);

            // Se não tem no cache, executa a factory function
            var value = await factory();

            // Configura opções de cache
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            // Salva no cache
            _cache.Set(key, value, options);
            _cacheKeys.Add(key);

            return value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _cacheKeys.Remove(key);
            _logger.LogDebug("Cache removido: {Key}", key);
        }

        public void RemoveByPrefix(string prefix)
        {
            var keysToRemove = _cacheKeys
                .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _cacheKeys.Remove(key);
            }

            _logger.LogDebug("Removidos {Count} registros do cache com prefixo: {Prefix}",
                keysToRemove.Count, prefix);
        }
    }
}