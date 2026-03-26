using MediatR;

namespace SmartPlatform.Application.Features.Reviews.Commands
{
    public record DeleteReviewCommand(int Id, string CustomerId) : IRequest;
}
