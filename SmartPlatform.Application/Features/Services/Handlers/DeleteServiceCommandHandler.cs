using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Services.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Services.Handlers
{
    public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public DeleteServiceCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.Repository<Service>().GetByIdAsync(request.Id);
            
            if (service == null) throw new Exception("Service not found");
            if (service.ProviderId != request.ProviderId) throw new UnauthorizedAccessException();

            _unitOfWork.Repository<Service>().Delete(service);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"ServiceDetails_{request.Id}");
            await _cacheService.RemoveAsync("DashboardStats_Admin_Global");
            await _cacheService.RemoveAsync($"DashboardStats_{request.ProviderId}_Admin_False");
            await _cacheService.RemoveAsync("Services_List_P1_S10_Call_Prall");
        }
    }
}
