using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Reviews.Queries;

namespace SmartPlatform.Application.Features.Reviews.Handlers
{
    public class GetReviewsByServiceQueryHandler : IRequestHandler<GetReviewsByServiceQuery, IEnumerable<ReviewDto>>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetReviewsByServiceQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<IEnumerable<ReviewDto>> Handle(GetReviewsByServiceQuery request, CancellationToken cancellationToken)
        {
            var sql = @"
                SELECT r.Id, r.Rating, r.Comment, r.ServiceRequestId,
                       u.FullName as CustomerName, u.Id as CustomerId
                FROM Reviews r
                JOIN ServiceRequests sr ON r.ServiceRequestId = sr.Id
                JOIN AspNetUsers u ON sr.CustomerId = u.Id
                WHERE sr.ServiceId = @ServiceId AND r.IsDeleted = 0";

            return await _readDbConnection.QueryAsync<ReviewDto>(sql, new { ServiceId = request.ServiceId });
        }
    }
}
