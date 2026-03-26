using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Platform.Models;
using X.PagedList;

namespace Smart_Platform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var query = _userManager.Users;
            int totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<ApplicationUser>(items, pageNumber, pageSize, totalCount);
            return View(pagedList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeProvider(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                if (!await _userManager.IsInRoleAsync(user, "Provider"))
                {
                    await _userManager.AddToRoleAsync(user, "Provider");
                }
                if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Customer");
                }
            }
            return RedirectToAction("Index");
        }
    }
}
