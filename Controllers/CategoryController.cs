using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Platform.Features.Categories.Commands.CreateCategory;
using Smart_Platform.Features.Categories.Commands.UpdateCategory;
using Smart_Platform.Features.Categories.Commands.DeleteCategory;
using Smart_Platform.Features.Categories.Queries.GetAllCategories;
using Smart_Platform.Features.Categories.Queries.GetCategoryById;
using Smart_Platform.ViewModel;

namespace Smart_Platform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;
            var categories = await _mediator.Send(new GetAllCategoriesQuery(pageNumber, pageSize));
            return View(categories);
        }

        public IActionResult Create()
        {
            return PartialView("_CreateEdit", new CategoryVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM category)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateEdit", category);
            }
            await _mediator.Send(new CreateCategoryCommand(category));

            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int Id)
        {
            var category = await _mediator.Send(new GetCategoryByIdQuery(Id));
            if (category == null) return NotFound();

            return PartialView("_CreateEdit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryVM category)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateEdit", category);
            }
            await _mediator.Send(new UpdateCategoryCommand(category));

            TempData["Success"] = "Category updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            await _mediator.Send(new DeleteCategoryCommand(Id));
            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
