using MediatR;
using Smart_Platform.Models;
using Smart_Platform.UOW;

namespace Smart_Platform.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<ServiceCategory>().GetByIdAsync(request.Id);
            if (category == null) return;

            _unitOfWork.Repository<ServiceCategory>().Delete(category);
            await _unitOfWork.CompleteAsync();
        }
    }
}
