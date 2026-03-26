using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class CompleteServiceRequestCommandHandler : IRequestHandler<CompleteServiceRequestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompleteServiceRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        }
    }
}
