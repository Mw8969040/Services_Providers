using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Services.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Services.Handlers
{
    public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly ICacheService _cacheService;

        public UpdateServiceCommandHandler(IUnitOfWork unitOfWork, IWebHostEnvironment env, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _cacheService = cacheService;
        }

        public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.Repository<Service>().GetByIdAsync(request.Id);
            
            if (service == null) throw new Exception("Service not found");
            if (service.ProviderId != request.ProviderId) throw new UnauthorizedAccessException();

            service.Title = request.ServiceDto.Title;
            service.Description = request.ServiceDto.Description;
            service.BasePrice = request.ServiceDto.BasePrice;
            service.IsAvailable = request.ServiceDto.IsAvailable;
            service.CategoryId = request.ServiceDto.CategoryId;

            if (request.ServiceDto.ImageFile != null)
            {
                service.ImageUrl = await SaveImageAsync(request.ServiceDto.ImageFile);
            }

            _unitOfWork.Repository<Service>().Update(service);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"ServiceDetails_{request.Id}");
            await _cacheService.RemoveAsync("DashboardStats_Admin_Global");
            await _cacheService.RemoveAsync($"DashboardStats_{request.ProviderId}_Admin_False");
            await _cacheService.RemoveAsync("Services_List_P1_S10_Call_Prall");
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "services");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return "/uploads/services/" + uniqueFileName;
        }
    }
}
