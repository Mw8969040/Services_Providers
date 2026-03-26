using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace SmartPlatform.Web.Filters
{
    public class CacheInvalidationFilter : Attribute, IActionFilter
    {
        private readonly  string[] _baseKey;

        public CacheInvalidationFilter(params string[] baseKey)
        {
            _baseKey = baseKey;
        }

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // نحذف الكاش فقط إذا نجحت عملية التعديل في قاعدة البيانات
            if (context.Exception == null)
            {
                var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
                string tokenKey = $"{_baseKey}_Token";

                if (cache.TryGetValue(tokenKey, out CancellationTokenSource cts))
                {
                    // إلغاء التوكن: يمسح جميع العناصر المرتبطة بهذا الـ BaseKey من الذاكرة فوراً
                    cts.Cancel();

                    // حذف التوكن نفسه لضمان إنشاء واحد جديد في الطلب القادم
                    cache.Remove(tokenKey);
                }
            }
        }
    }
}

