using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Reviews.Queries
{
    public record GetReviewsByServiceQuery(int ServiceId) : IRequest<IEnumerable<ReviewDto>>;
}
