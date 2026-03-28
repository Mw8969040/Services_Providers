using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Queries
{
    public class GetServiceByIdQuery : IRequest<ServiceDto?>
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }

        public GetServiceByIdQuery(int id, string? customerId = null)
        {
            Id = id;
            CustomerId = customerId;
        }
    }
}
