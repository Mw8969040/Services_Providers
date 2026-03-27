using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Services.Commands;
using SmartPlatform.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace SmartPlatform.Application.Features.Services.Handlers
{
    public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public CreateServiceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        public async Task Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = _mapper.Map<Service>(request.ServiceDto);
            service.ProviderId = request.ProviderId;

            if (request.ServiceDto.ImageFile != null)
            {
                service.ImageUrl = await SaveImageAsync(request.ServiceDto.ImageFile);
            }

            await _unitOfWork.Repository<Service>().AddAsync(service);
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
