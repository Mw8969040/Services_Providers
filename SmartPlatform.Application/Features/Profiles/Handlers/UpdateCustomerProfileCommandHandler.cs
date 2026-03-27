using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Profiles.Commands;
using SmartPlatform.Domain.Entities;
using System.Linq;

namespace SmartPlatform.Application.Features.Profiles.Handlers
{
    public class UpdateCustomerProfileCommandHandler : IRequestHandler<UpdateCustomerProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.Repository<CustomerProfile>()
                .GetByIdWithIncludesAsync(p => p.Id == request.Profile.Id);

            if (profile == null) return false;

            profile.FullName = request.Profile.FullName;
            profile.Address = request.Profile.Address;
            profile.ProfilePictureUrl = request.Profile.ProfilePictureUrl;

            _unitOfWork.Repository<CustomerProfile>().Update(profile);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
