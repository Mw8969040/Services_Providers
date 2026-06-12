using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartPlatform.Application.Common.Interfaces;

namespace SmartPlatform.Infrastructure.Services
{
    public sealed class MemoryCacheService : ICacheService, IDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly bool _enabled;

        private readonly Dictionary<string, HashSet<string>> _groupToKeys = new();
        private readonly ConcurrentDictionary<string, string> _keyToGroup = new();

        private readonly SemaphoreSlim _groupLock = new(1, 1);

        private readonly ConcurrentDictionary<string, RefCountedSemaphore> _keyLocks = new();

        private class RefCountedSemaphore
        {
            public readonly SemaphoreSlim Semaphore = new(1, 1);
            public int RefCount = 0;
        }

        public MemoryCacheService(
            IMemoryCache cache,
            IConfiguration configuration,
            ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            _enabled = configuration.GetValue<bool>("EnableCaching", true);
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_enabled)
            {
                _logger.LogDebug("[Cache] Disabled — skipping GET '{Key}'", key);
                return Task.FromResult(default(T));
            }

            if (_cache.TryGetValue(key, out T? value))
            {
                _logger.LogDebug("[Cache] HIT '{Key}'", key);
                return Task.FromResult(value);
            }

            _logger.LogDebug("[Cache] MISS '{Key}'", key);
            return Task.FromResult(default(T));
        }

        public async Task SetAsync<T>(
                    string key,
                    T value,
                    TimeSpan absoluteExpiration,
                    string? group = null,
                    TimeSpan? slidingExpiration = null,
                    CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_enabled)
            {
                _logger.LogDebug("[Cache] Disabled — skipping SET '{Key}'", key);
                return;
            }

            _cache.Set(key, value, BuildOptions(absoluteExpiration, slidingExpiration));

            _logger.LogDebug(
                "[Cache] SET '{Key}' | Absolute={Abs} | Sliding={Slide} | Group='{Group}'",
                key, absoluteExpiration, slidingExpiration?.ToString() ?? "none", group ?? "none");

            if (group is not null)
                await RegisterInGroupAsync(key, group, cancellationToken);
        }

        public async Task<T> GetOrCreateAsync<T>(
                    string key,
                    Func<CancellationToken, Task<T>> factory,
                    TimeSpan absoluteExpiration,
                    string? group = null,
                    TimeSpan? slidingExpiration = null,
                    CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!_enabled)
            {
                _logger.LogDebug("[Cache] Disabled — running factory directly for '{Key}'", key);
                return await factory(cancellationToken);
            }

            if (_cache.TryGetValue(key, out T? cached))
            {
                _logger.LogDebug("[Cache] HIT (GetOrCreate) '{Key}'", key);
                return cached!;
            }

            _logger.LogDebug("[Cache] MISS (GetOrCreate) '{Key}' — acquiring per-key lock", key);

            RefCountedSemaphore refLock;
            lock (_keyLocks)
            {
                refLock = _keyLocks.GetOrAdd(key, _ => new RefCountedSemaphore());
                refLock.RefCount++;
            }

            await refLock.Semaphore.WaitAsync(cancellationToken);

            try
            {
                if (_cache.TryGetValue(key, out cached))
                {
                    _logger.LogDebug("[Cache] HIT (GetOrCreate double-check) '{Key}'", key);
                    return cached!;
                }

                _logger.LogDebug("[Cache] Executing factory for '{Key}'", key);
                var value = await factory(cancellationToken);

                await SetAsync(key, value, absoluteExpiration, group, slidingExpiration, cancellationToken);

                return value;
            }
            finally
            {
                refLock.Semaphore.Release();
                lock (_keyLocks)
                {
                    refLock.RefCount--;
                    if (refLock.RefCount == 0)
                    {
                        if (_keyLocks.TryRemove(key, out var removed) && removed != refLock)
                        {
                            _keyLocks.TryAdd(key, removed);
                        }
                        refLock.Semaphore.Dispose();
                    }
                }
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_keyToGroup.TryRemove(key, out var group))
            {
                await _groupLock.WaitAsync(cancellationToken);
                try
                {
                    if (_groupToKeys.TryGetValue(group, out var keys))
                    {
                        keys.Remove(key);

                        if (keys.Count == 0)
                        {
                            _groupToKeys.Remove(group);
                            _logger.LogDebug(
                                "[Cache] Group '{Group}' auto-removed (empty after RemoveAsync)", group);
                        }
                    }
                }
                finally
                {
                    _groupLock.Release();
                }
            }

            _cache.Remove(key);
            _logger.LogDebug("[Cache] REMOVE '{Key}'", key);
        }

        public async Task RemoveGroupAsync(string group, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _groupLock.WaitAsync(cancellationToken);

            string[] snapshot;
            try
            {
                if (!_groupToKeys.TryGetValue(group, out var keys))
                {
                    _logger.LogDebug("[Cache] RemoveGroup '{Group}' — group not found, nothing to do", group);
                    return;
                }

                snapshot = keys.ToArray();

                foreach (var key in snapshot)
                    _keyToGroup.TryRemove(key, out _);

                _groupToKeys.Remove(group);
            }
            finally
            {
                _groupLock.Release();
            }

            foreach (var key in snapshot)
                _cache.Remove(key);

            _logger.LogInformation(
                "[Cache] GROUP REMOVED '{Group}' — {Count} entries invalidated: [{Keys}]",
                group, snapshot.Length, string.Join(", ", snapshot));
        }

        private MemoryCacheEntryOptions BuildOptions(TimeSpan absoluteExpiration, TimeSpan? slidingExpiration)
        {
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(absoluteExpiration)
                .RegisterPostEvictionCallback(OnEntryEvicted);

            if (slidingExpiration.HasValue)
                options.SetSlidingExpiration(slidingExpiration.Value);

            return options;
        }

        private async Task RegisterInGroupAsync(string key, string group, CancellationToken cancellationToken)
        {
            await _groupLock.WaitAsync(cancellationToken);
            try
            {
                if (!_groupToKeys.TryGetValue(group, out var keys))
                {
                    keys = new HashSet<string>();
                    _groupToKeys[group] = keys;
                }

                keys.Add(key);
                _keyToGroup[key] = group;

                _logger.LogDebug(
                    "[Cache] Registered '{Key}' → group '{Group}' ({Count} keys in group)",
                    key, group, keys.Count);
            }
            finally
            {
                _groupLock.Release();
            }
        }

        private void OnEntryEvicted(object rawKey, object? value, EvictionReason reason, object? state)
        {
            if (reason == EvictionReason.Replaced) return;

            var key = rawKey.ToString()!;
            _logger.LogDebug("[Cache] EVICTED '{Key}' (Reason: {Reason})", key, reason);

            if (!_keyToGroup.TryRemove(key, out var group)) return;

            _groupLock.Wait();
            try
            {
                if (!_groupToKeys.TryGetValue(group, out var keys)) return;

                keys.Remove(key);

                if (keys.Count == 0)
                {
                    _groupToKeys.Remove(group);
                    _logger.LogDebug(
                        "[Cache] Group '{Group}' auto-removed (empty after eviction)", group);
                }
            }
            finally
            {
                _groupLock.Release();
            }
        }

        public void Dispose()
        {
            _groupLock.Dispose();

            foreach (var sem in _keyLocks.Values)
                sem.Semaphore.Dispose();
        }
    }
}
