using MediatR;
using Microsoft.Extensions.Logging;
using SmartPlatform.Application.Common.Interfaces;

namespace SmartPlatform.Application.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // If the request doesn't implement ICacheableQuery, just proceed normally
            if (request is not ICacheableQuery cacheableQuery)
            {
                return await next();
            }

            var cacheKey = cacheableQuery.CacheKey;
            
            // Try fetch from cache
            var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey);
            if (cachedResponse != null)
            {
                _logger.LogInformation("Cache MATCH for {CacheKey}. Returning cached response.", cacheKey);
                return cachedResponse;
            }

            _logger.LogInformation("Cache MISS for {CacheKey}. Fetching from database...", cacheKey);
            
            // Execute the actual handler if not in cache
            var response = await next();

            // Store in cache
            if (response != null)
            {
                var expiration = TimeSpan.FromMinutes(cacheableQuery.CacheTimeInMinutes);
                await _cacheService.SetAsync(cacheKey, response, expiration);
                _logger.LogInformation("Stored {CacheKey} in cache for {Minutes} minutes.", cacheKey, cacheableQuery.CacheTimeInMinutes);
            }

            return response;
        }
    }
}
