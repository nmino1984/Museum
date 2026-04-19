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

            RuleFor(x => x.Theme)
                .InclusiveBetween(1, 3).WithMessage("Theme must be 1 (Art), 2 (Natural Sciences) or 3 (History)");
        }
    }
}
