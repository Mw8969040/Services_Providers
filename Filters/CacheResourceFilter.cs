using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Smart_Platform.Filters
{
    public class CacheResourceFilter : Attribute, IResourceFilter
    {
        private readonly string _baseKey;
        private readonly int _durationInMinutes;

        public CacheResourceFilter(string baseKey, int durationInMinutes = 10)
        {
            _baseKey = baseKey;
            _durationInMinutes = durationInMinutes;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            // تصحيح: تمرير الـ HttpContext مباشرة بدلاً من الـ FilterContext كاملاً
            string fullKey = GenerateKey(context.HttpContext);

            if (cache.TryGetValue(fullKey, out IActionResult cachedResult))
            {
                context.Result = cachedResult;
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            if (context.Exception == null && context.Result != null)
            {
                var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

                // تصحيح: تمرير الـ HttpContext
                string fullKey = GenerateKey(context.HttpContext);

                string tokenKey = $"{_baseKey}_Token";
                var cts = cache.GetOrCreate(tokenKey, entry => new CancellationTokenSource());

                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(_durationInMinutes))
                    .AddExpirationToken(new CancellationChangeToken(cts.Token));

                cache.Set(fullKey, context.Result, options);
            }
        }

        // الحل الجذري: تغيير المدخلات لـ HttpContext عشان تنفع للـ Executing والـ Executed
        private string GenerateKey(HttpContext httpContext)
        {
            var request = httpContext.Request;
            return $"{_baseKey}_{request.Path}_{request.QueryString}";
        }
    }
}