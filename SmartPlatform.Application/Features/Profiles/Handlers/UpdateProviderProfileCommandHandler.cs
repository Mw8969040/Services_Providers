using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Profiles.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Profiles.Handlers
{
    public class UpdateProviderProfileCommandHandler : IRequestHandler<UpdateProviderProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public UpdateProviderProfileCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
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
            var result = await _unitOfWork.CompleteAsync() > 0;

            if (result)
            {
                // Invalidate Cache
                await _cacheService.RemoveAsync("Services_List_P1_S10_C0_Prall");
                await _cacheService.RemoveAsync($"Services_List_P1_S10_C0_Pr{profile.UserId}");
                await _cacheService.RemoveAsync($"DashboardStats_{profile.UserId}_Admin_False");
            }

            return result;
        }
    }
}
