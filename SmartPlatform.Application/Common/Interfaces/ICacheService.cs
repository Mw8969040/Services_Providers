namespace SmartPlatform.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        Task SetAsync<T>(
            string key,
            T value,
            TimeSpan absoluteExpiration,
            string? group = null,
            TimeSpan? slidingExpiration = null,
            CancellationToken cancellationToken = default);

        Task<T> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            TimeSpan absoluteExpiration,
            string? group = null,
            TimeSpan? slidingExpiration = null,
            CancellationToken cancellationToken = default);

        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        Task RemoveGroupAsync(string group, CancellationToken cancellationToken = default);
    }
}
