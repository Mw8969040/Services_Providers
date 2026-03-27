using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Profiles.Queries
{
    public record GetPublicProfileQuery(string ProviderId) : IRequest<ProviderProfileDto?>;
}
