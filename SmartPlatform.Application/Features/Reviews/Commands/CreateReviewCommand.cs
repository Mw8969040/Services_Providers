using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Reviews.Commands
{
    public record CreateReviewCommand(ReviewVM ReviewVM) : IRequest;
}
