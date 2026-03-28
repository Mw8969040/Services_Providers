using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Categories.Commands;
using SmartPlatform.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SmartPlatform.Application.Features.Categories.Handlers
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly ICacheService _cacheService;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IWebHostEnvironment env, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _cacheService = cacheService;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<ServiceCategory>().GetByIdAsync(request.CategoryDto.Id);
            
            if (category == null) throw new Exception("Category not found");

            category.Name = request.CategoryDto.Name;
            category.Description = request.CategoryDto.Description;

            if (request.CategoryDto.ImageFile != null)
            {
                category.ImageUrl = await SaveImageAsync(request.CategoryDto.ImageFile);
            }

            _unitOfWork.Repository<ServiceCategory>().Update(category);
            await _unitOfWork.CompleteAsync();

            // Invalidate Cache
            await _cacheService.RemoveAsync("Services_List_P1_S10_C0_Prall");
            await _cacheService.RemoveAsync($"Services_List_P1_S10_C{category.Id}_Prall");
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "categories");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return "/uploads/categories/" + uniqueFileName;
        }
    }
}
