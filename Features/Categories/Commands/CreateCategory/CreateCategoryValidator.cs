using FluentValidation;
using Smart_Platform.ViewModel;

namespace Smart_Platform.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.CategoryVM.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
        }
    }
}
