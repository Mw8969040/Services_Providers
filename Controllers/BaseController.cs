using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace SmartPlatform.Web.Controllers
{
    public class BaseController :Controller
    {
        protected string userId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        protected bool IsProvider => User.IsInRole("Provider");
        protected bool IsCustomer => User.IsInRole("Customer");
        protected bool IsAdmin => User.IsInRole("Admin");
    }
}
