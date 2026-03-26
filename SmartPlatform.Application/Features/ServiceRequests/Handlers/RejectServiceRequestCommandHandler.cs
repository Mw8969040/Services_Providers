using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class RejectServiceRequestCommandHandler : IRequestHandler<RejectServiceRequestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RejectServiceRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RejectServiceRequestCommand request, CancellationToken cancellationToken)
        {
            var serviceRequest = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(r => r.Id == request.RequestId, "Service");
            
            if (serviceRequest == null) throw new Exception("Service Request not found");
            if (serviceRequest.Service.ProviderId != request.ProviderId) throw new UnauthorizedAccessException();

            serviceRequest.requestStatus = RequestStatus.Rejected;
            _unitOfWork.Repository<ServiceRequest>().Update(serviceRequest);
            await _unitOfWork.CompleteAsync();
        }
    }
}
