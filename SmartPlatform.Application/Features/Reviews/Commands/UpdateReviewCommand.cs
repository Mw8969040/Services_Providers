using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Reviews.Commands
{
    public record UpdateReviewCommand(ReviewVM ReviewVM, string CustomerId) : IRequest;
}
