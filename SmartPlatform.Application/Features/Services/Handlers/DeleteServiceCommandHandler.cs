using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Services.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Services.Handlers
{
    public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteServiceCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.Repository<Service>().GetByIdAsync(request.Id);
            
            if (service == null) throw new Exception("Service not found");
            if (service.ProviderId != request.ProviderId) throw new UnauthorizedAccessException();

            _unitOfWork.Repository<Service>().Delete(service);
            await _unitOfWork.CompleteAsync();
        }
    }
}
