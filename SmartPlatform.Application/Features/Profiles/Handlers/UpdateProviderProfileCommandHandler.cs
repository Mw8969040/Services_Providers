using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Profiles.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Profiles.Handlers
{
    public class UpdateProviderProfileCommandHandler : IRequestHandler<UpdateProviderProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProviderProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateProviderProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = await _unitOfWork.Repository<ProviderProfile>()
                .GetByIdWithIncludesAsync(p => p.Id == request.Profile.Id);

            if (profile == null) return false;

            profile.BusinessName = request.Profile.BusinessName;
            profile.ProviderName = request.Profile.ProviderName;
            profile.Description = request.Profile.Description;
            profile.ProfilePictureUrl = request.Profile.ProfilePictureUrl;
            profile.YearsOfExperience = request.Profile.YearsOfExperience;

            _unitOfWork.Repository<ProviderProfile>().Update(profile);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
