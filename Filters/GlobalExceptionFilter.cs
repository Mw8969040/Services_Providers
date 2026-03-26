using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SmartPlatform.Web.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public GlobalExceptionFilter(ITempDataDictionaryFactory tempDataFactory)
        {
            _tempDataFactory = tempDataFactory;
        }

        public void OnException(ExceptionContext context)
        {
            // 1. Get TempData
            var tempData = _tempDataFactory.GetTempData(context.HttpContext);

            // 2. Set the error message
            tempData["Error"] = context.Exception.Message;

            // 3. Determine where to redirect (Referer is the page the user was on)
            string returnUrl = context.HttpContext.Request.Headers["Referer"].ToString();

            if (string.IsNullOrEmpty(returnUrl))
            {
                // Fallback to Home if no Referer is present
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
            else
            {
                // Redirect back to original page with error message in TempData
                context.Result = new RedirectResult(returnUrl);
            }

            // 4. Mark exception as handled
            context.ExceptionHandled = true;
        }
    }
}
