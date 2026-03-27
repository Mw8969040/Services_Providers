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
            var serviceRequest = _mapper.Map<ServiceRequest>(request.ServiceRequestDto);
            serviceRequest.RequestDate = DateTime.Now;
            serviceRequest.requestStatus = RequestStatus.Pending;

            await _unitOfWork.Repository<ServiceRequest>().AddAsync(serviceRequest);
            await _unitOfWork.CompleteAsync();
        }
    }
}
