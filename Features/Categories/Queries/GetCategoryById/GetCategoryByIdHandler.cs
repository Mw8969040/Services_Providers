using AutoMapper;
using MediatR;
using Smart_Platform.Models;
using Smart_Platform.UOW;
using Smart_Platform.ViewModel;

namespace Smart_Platform.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryVM?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryVM?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<ServiceCategory>().GetByIdAsync(request.Id);
            return category == null ? null : _mapper.Map<CategoryVM>(category);
        }
    }
}
