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

        public UpdateServiceCommandHandler(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.Repository<Service>().GetByIdAsync(request.Id);
            
            if (service == null) throw new Exception("Service not found");
            if (service.ProviderId != request.ProviderId) throw new UnauthorizedAccessException();

            service.Title = request.ServiceVM.Title;
            service.Description = request.ServiceVM.Description;
            service.BasePrice = request.ServiceVM.BasePrice;
            service.IsAvailable = request.ServiceVM.IsAvailable;
            service.CategoryId = request.ServiceVM.CategoryId;

            if (request.ServiceVM.ImageFile != null)
            {
                service.ImageUrl = await SaveImageAsync(request.ServiceVM.ImageFile);
            }

            _unitOfWork.Repository<Service>().Update(service);
            await _unitOfWork.CompleteAsync();
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
