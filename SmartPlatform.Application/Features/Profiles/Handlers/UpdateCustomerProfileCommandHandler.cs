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
        private readonly ICacheService _cacheService;

        public UpdateCustomerProfileCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
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
            var result = await _unitOfWork.CompleteAsync() > 0;

            if (result)
            {
                // Invalidate Cache
                await _cacheService.RemoveAsync($"ServiceRequests_List_P1_S10_Prall_Cu{profile.UserId}_Schnone_Bynone");
                await _cacheService.RemoveAsync($"DashboardStats_{profile.UserId}_Admin_False");
            }

            return result;
        }
    }
}
