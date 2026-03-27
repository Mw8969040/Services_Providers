using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Profiles.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Profiles.Handlers
{
    public class CreateProviderProfileCommandHandler : IRequestHandler<CreateProviderProfileCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProviderProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CreateProviderProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = new ProviderProfile
            {
                UserId = request.UserId,
                BusinessName = request.BusinessName,
                ProviderName = request.ProviderName,
                Description = request.Description,
                ProfilePictureUrl = request.ProfilePictureUrl,
                Rating = 0, // Initial rating
                YearsOfExperience = request.YearsOfExperience
            };

            await _unitOfWork.Repository<ProviderProfile>().AddAsync(profile);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
