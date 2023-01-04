using FluentValidation;
using Application.ViewModels.Request;

namespace Application.Validators
{
    public class ArticleValidator : AbstractValidator<ArticleRequestViewModel>
    {
        public ArticleValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Name can't be Null")
                .NotEmpty().WithMessage("Name can't be Empty");
        }
    }
}
