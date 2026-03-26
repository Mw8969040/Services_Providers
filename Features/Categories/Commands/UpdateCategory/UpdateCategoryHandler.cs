using AutoMapper;
using MediatR;
using Smart_Platform.Models;
using Smart_Platform.UOW;

namespace Smart_Platform.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<ServiceCategory>().GetByIdAsync(request.CategoryVM.Id);
            if (category == null) return;

            category.Name = request.CategoryVM.Name;
            category.Description = request.CategoryVM.Description;
            // Image handling would go here if needed.

            _unitOfWork.Repository<ServiceCategory>().Update(category);
            await _unitOfWork.CompleteAsync();
        }
    }
}
