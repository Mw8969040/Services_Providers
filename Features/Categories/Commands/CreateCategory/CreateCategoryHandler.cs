using AutoMapper;
using MediatR;
using Smart_Platform.Models;
using Smart_Platform.UOW;

namespace Smart_Platform.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = _mapper.Map<ServiceCategory>(request.CategoryVM);
            // Note: Image saving logic could be added here or handled in a shared utility.
            // For now, mapping as per existing logic in CategoryService.
            await _unitOfWork.Repository<ServiceCategory>().AddAsync(category);
            await _unitOfWork.CompleteAsync();
        }
    }
}
