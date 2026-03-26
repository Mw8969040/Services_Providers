using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Smart_Platform.Controllers
{
    public class BaseController :Controller
    {
        protected string userId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        protected bool IsProvider => User.IsInRole("Provider");
        protected bool IsCustomer => User.IsInRole("Customer");
        protected bool IsAdmin => User.IsInRole("Admin");
    }
}
