using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Categories.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Categories.Handlers
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<ServiceCategory>().GetByIdAsync(request.Id);
            
            if (category == null) throw new Exception("Category not found");

            _unitOfWork.Repository<ServiceCategory>().Delete(category);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveGroupAsync("Categories", cancellationToken);
        }
    }
}
