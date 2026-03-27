using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Profiles.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Profiles.Handlers
{
    public class CreateCustomerProfileCommandHandler : IRequestHandler<CreateCustomerProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CreateCustomerProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = new CustomerProfile
            {
                UserId = request.UserId,
                FullName = request.FullName,
                Address = request.Address,
                ProfilePictureUrl = request.ProfilePictureUrl
            };

            await _unitOfWork.Repository<CustomerProfile>().AddAsync(profile);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
