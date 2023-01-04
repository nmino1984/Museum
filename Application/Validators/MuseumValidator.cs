using FluentValidation;
using Application.ViewModels.Request;

namespace Application.Validators
{
    public class MuseumValidator : AbstractValidator<MuseumRequestViewModel>
    {
        public MuseumValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Name can't be Null")
                .NotEmpty().WithMessage("Name can't be Empty");
        }
    }
}
