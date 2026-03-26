using MediatR;

namespace SmartPlatform.Application.Features.Admin.Commands
{
    public record MakeProviderCommand(string UserId) : IRequest;
}
