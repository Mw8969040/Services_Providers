using MediatR;
using AutoMapper;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Categories.Queries;
using SmartPlatform.Application.Features.Categories.Commands;
using SmartPlatform.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SmartPlatform.Application.Features.Categories.Handlers
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ICacheService _cacheService;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
            _cacheService = cacheService;
        }

        public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = _mapper.Map<ServiceCategory>(request.CategoryDto);

            if (request.CategoryDto.ImageFile != null)
            {
                category.ImageUrl = await SaveImageAsync(request.CategoryDto.ImageFile);
            }

            await _unitOfWork.Repository<ServiceCategory>().AddAsync(category);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveGroupAsync("Categories", cancellationToken);
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
