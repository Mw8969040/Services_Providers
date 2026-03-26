using AutoMapper;
using MediatR;
using Smart_Platform.Models;
using Smart_Platform.UOW;
using Smart_Platform.ViewModel;
using X.PagedList;

namespace Smart_Platform.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, IPagedList<CategoryVM>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategoriesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IPagedList<CategoryVM>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Repository<ServiceCategory>().GetPagedWithIncludesAsync(request.PageNumber, request.PageSize, null, "Services");
            var mappedItems = _mapper.Map<IEnumerable<CategoryVM>>(categories);
            return new StaticPagedList<CategoryVM>(mappedItems, categories);
        }
    }
}
