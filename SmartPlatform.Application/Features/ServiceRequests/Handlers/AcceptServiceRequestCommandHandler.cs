using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class AcceptServiceRequestCommandHandler : IRequestHandler<AcceptServiceRequestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public AcceptServiceRequestCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task Handle(AcceptServiceRequestCommand request, CancellationToken cancellationToken)
        {
            var serviceRequest = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(r => r.Id == request.RequestId, "Service");
            
            if (serviceRequest == null) throw new Exception("Service Request not found");
            if (serviceRequest.Service.ProviderId != request.ProviderId) throw new UnauthorizedAccessException();

            serviceRequest.requestStatus = RequestStatus.Accepted;
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
