using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Admin.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Admin.Handlers
{
    public class MakeProviderCommandHandler : IRequestHandler<MakeProviderCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public MakeProviderCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(MakeProviderCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) throw new Exception("User not found");

            if (!await _userManager.IsInRoleAsync(user, "Provider"))
            {
                await _userManager.AddToRoleAsync(user, "Provider");
                
                // Automatically provision ProviderProfile for the new Provider
                var existingProfile = await _unitOfWork.Repository<ProviderProfile>().GetByIdWithIncludesAsync(p => p.UserId == user.Id);
                if (existingProfile == null)
                {
                    await _unitOfWork.Repository<ProviderProfile>().AddAsync(new ProviderProfile 
                    { 
                        UserId = user.Id,
                        BusinessName = "General Service", // Default Specialty
                        ProviderName = user.FullName,
                        ProfilePictureUrl = null,
                        Description = "Professional service provider on Smart Platform",
                        Rating = 0
                    });
                    
                    await _unitOfWork.CompleteAsync();
                }
            }

            if (await _userManager.IsInRoleAsync(user, "Customer"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Customer");
                var customerProfile = await _unitOfWork.Repository<CustomerProfile>().GetByIdWithIncludesAsync(p => p.UserId == user.Id);
                if (customerProfile != null)
                {
                    _unitOfWork.Repository<CustomerProfile>().Delete(customerProfile);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }
    }
}
