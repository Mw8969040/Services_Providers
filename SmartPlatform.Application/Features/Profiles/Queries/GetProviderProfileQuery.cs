using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Profiles.Queries
{
    public record GetProviderProfileQuery(string UserId) : IRequest<ProviderProfileDto?>;
}
