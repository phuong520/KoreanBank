using FluentValidation;
using KEB.Domain.Entities;
using KEB.Domain.ValueObject;

namespace FSAKEB.Application.Extensions.FluentValidationRules.ReferenceValidators
{
    public class ReferenceValidator : AbstractValidator<References>
    {
        public ReferenceValidator()
        {
            RuleFor(x => x.ReferenceName).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(AppMessages.EMPTY_REFERENCE);
            RuleFor(x => x.ReferenceAuthor).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(AppMessages.EMPTY_AUTHOR);
            RuleFor(x => x.PublishedYear)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(AppMessages.EMPTY_PUBLISHED_YEAR)
                .Must(year => year <= DateTime.Now.Year)
                .WithMessage(AppMessages.INVALID_REFERENCE_YEAR);
            RuleFor(x => x.ReferencesLink).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(AppMessages.EMPTY_REFERENCE_LINK);
        }
    }
}
