using MediatR;
using AutoMapper;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateServiceRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(CreateServiceRequestCommand request, CancellationToken cancellationToken)
        {
            var service = await _unitOfWork.Repository<Service>().GetByIdAsync(request.ServiceRequestDto.ServiceId);
            if (service == null) throw new Exception("Service not found");

            var customerProfile = await _unitOfWork.Repository<CustomerProfile>()
                .GetByIdWithIncludesAsync(cp => cp.UserId == request.ServiceRequestDto.CustomerId, "User");

            // Check for existing active requests (Pending or Accepted)
            var existingRequest = await _unitOfWork.Repository<ServiceRequest>()
                .GetByIdWithIncludesAsync(sr => 
                    sr.ServiceId == request.ServiceRequestDto.ServiceId && 
                    sr.CustomerId == request.ServiceRequestDto.CustomerId && 
                    (sr.requestStatus == RequestStatus.Pending || sr.requestStatus == RequestStatus.Accepted) &&
                    sr.IsDeleted == false
                );

            if (existingRequest != null)
            {
                throw new Exception("You already have an active request for this service.");
            }

            var serviceRequest = _mapper.Map<ServiceRequest>(request.ServiceRequestDto);
            serviceRequest.RequestDate = DateTime.Now;
            serviceRequest.requestStatus = RequestStatus.Pending;
            serviceRequest.TotalPrice = service.BasePrice;

            // Optional: Auto-fill customer info from profile if not provided in DTO
            if (string.IsNullOrEmpty(serviceRequest.CustomerPhoneNumber) && customerProfile?.User != null)
            {
                serviceRequest.CustomerPhoneNumber = customerProfile.User.PhoneNumber;
            }

            if (string.IsNullOrEmpty(serviceRequest.CustomerAddress) && customerProfile != null)
            {
                serviceRequest.CustomerAddress = customerProfile.Address;
            }

            await _unitOfWork.Repository<ServiceRequest>().AddAsync(serviceRequest);
            await _unitOfWork.CompleteAsync();
        }
    }
}
