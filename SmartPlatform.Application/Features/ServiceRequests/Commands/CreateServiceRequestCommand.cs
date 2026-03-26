using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.ServiceRequests.Commands
{
    public record CreateServiceRequestCommand(ServiceRequestVM ServiceRequestVM) : IRequest;
}
