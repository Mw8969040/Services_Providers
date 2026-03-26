using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class CancelServiceRequestCommandHandler : IRequestHandler<CancelServiceRequestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelServiceRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CancelServiceRequestCommand request, CancellationToken cancellationToken)
        {
            var serviceRequest = await _unitOfWork.Repository<ServiceRequest>().GetByIdAsync(request.RequestId);
            
            if (serviceRequest == null) throw new Exception("Service Request not found");
            if (serviceRequest.CustomerId != request.CustomerId) throw new UnauthorizedAccessException();
            if (serviceRequest.requestStatus != RequestStatus.Pending) throw new Exception("Only pending requests can be cancelled");

            serviceRequest.requestStatus = RequestStatus.Cancelled;
            _unitOfWork.Repository<ServiceRequest>().Update(serviceRequest);
            await _unitOfWork.CompleteAsync();
        }
    }
}
