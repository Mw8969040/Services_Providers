using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class CancelServiceRequestCommandHandler : IRequestHandler<CancelServiceRequestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public CancelServiceRequestCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task Handle(CancelServiceRequestCommand request, CancellationToken cancellationToken)
        {
            var serviceRequest = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(r => r.Id == request.RequestId, "Service");
            
            if (serviceRequest == null) throw new Exception("Service Request not found");
            if (serviceRequest.CustomerId != request.CustomerId) throw new UnauthorizedAccessException();
            if (serviceRequest.requestStatus != RequestStatus.Pending) throw new Exception("Only pending requests can be cancelled");

            serviceRequest.requestStatus = RequestStatus.Cancelled;
            _unitOfWork.Repository<ServiceRequest>().Update(serviceRequest);
            await _unitOfWork.CompleteAsync();

            // Invalidate Cache
            await _cacheService.RemoveAsync($"ServiceRequests_List_P1_S10_Prall_Cu{serviceRequest.CustomerId}_Schnone_Bynone");
            await _cacheService.RemoveAsync($"ServiceRequests_List_P1_S10_Pr{serviceRequest.Service.ProviderId}_Cuall_Schnone_Bynone");
            await _cacheService.RemoveAsync($"DashboardStats_{serviceRequest.CustomerId}_Admin_False");
            await _cacheService.RemoveAsync($"DashboardStats_{serviceRequest.Service.ProviderId}_Admin_False");
            await _cacheService.RemoveAsync("DashboardStats_Admin_Global");
        }
    }
}
