using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class CompleteServiceRequestCommandHandler : IRequestHandler<CompleteServiceRequestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public CompleteServiceRequestCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task Handle(CompleteServiceRequestCommand request, CancellationToken cancellationToken)
        {
            var serviceRequest = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(r => r.Id == request.RequestId, "Service");
            
            if (serviceRequest == null) throw new Exception("Service Request not found");
            if (serviceRequest.Service.ProviderId != request.ProviderId || serviceRequest.requestStatus != RequestStatus.Accepted) 
                throw new UnauthorizedAccessException();

            serviceRequest.requestStatus = RequestStatus.Completed;
            _unitOfWork.Repository<ServiceRequest>().Update(serviceRequest);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveGroupAsync("ServiceRequests", cancellationToken);
            await _cacheService.RemoveGroupAsync("DashboardStats", cancellationToken);
            await _cacheService.RemoveAsync($"ServiceDetails_{serviceRequest.ServiceId}");
        }
    }
}
