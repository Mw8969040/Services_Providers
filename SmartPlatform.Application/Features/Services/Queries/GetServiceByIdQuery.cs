using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Queries
{
    public class GetServiceByIdQuery : IRequest<ServiceVM?>
    {
        public int Id { get; set; }

        public GetServiceByIdQuery(int id)
        {
            Id = id;
        }
    }
}
