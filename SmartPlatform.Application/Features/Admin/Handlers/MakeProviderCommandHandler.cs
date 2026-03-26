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

        public MakeProviderCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(MakeProviderCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) throw new Exception("User not found");

            if (!await _userManager.IsInRoleAsync(user, "Provider"))
            {
                await _userManager.AddToRoleAsync(user, "Provider");
            }

            if (await _userManager.IsInRoleAsync(user, "Customer"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Customer");
            }
        }
    }
}
