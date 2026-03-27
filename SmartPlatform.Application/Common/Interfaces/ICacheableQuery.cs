namespace SmartPlatform.Application.Common.Interfaces
{
    public interface ICacheableQuery
    {
        string CacheKey { get; }
        int CacheTimeInMinutes { get; }
    }
}
