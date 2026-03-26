using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class AcceptServiceRequestCommandHandler : IRequestHandler<AcceptServiceRequestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AcceptServiceRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AcceptServiceRequestCommand request, CancellationToken cancellationToken)
        {
            var serviceRequest = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(r => r.Id == request.RequestId, "Service");
            
            if (serviceRequest == null) throw new Exception("Service Request not found");
            if (serviceRequest.Service.ProviderId != request.ProviderId) throw new UnauthorizedAccessException();

            serviceRequest.requestStatus = RequestStatus.Accepted;
            _unitOfWork.Repository<ServiceRequest>().Update(serviceRequest);
            await _unitOfWork.CompleteAsync();
        }
    }
}
