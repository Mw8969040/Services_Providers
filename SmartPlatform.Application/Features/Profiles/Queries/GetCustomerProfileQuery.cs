using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Profiles.Queries
{
    public record GetCustomerProfileQuery(string UserId) : IRequest<CustomerProfileDto?>;
}
