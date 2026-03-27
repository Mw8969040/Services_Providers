using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Queries
{
    public class GetServiceByIdQuery : IRequest<ServiceDto?>, SmartPlatform.Application.Common.Interfaces.ICacheableQuery
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; }

        public string CacheKey => $"Service_{Id}_{CustomerId ?? "anon"}";

        public int CacheTimeInMinutes => 10;

        public GetServiceByIdQuery(int id, string? customerId = null)
        {
            Id = id;
            CustomerId = customerId;
        }
    }
}
