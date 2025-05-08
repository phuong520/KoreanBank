using FluentValidation;
using KEB.Application.DTOs.UserDTO;
using KEB.Domain.ValueObject;

namespace FSAKEB.Application.Extensions.FluentValidationRules
{
    public class UserUpdateRequestValidator : AbstractValidator<UpdateUser>
    {
        public UserUpdateRequestValidator()
        {
            //RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
            //    .EmailAddress()
            //    .WithName("Email").WithMessage(LocalizationString.Validator.EmailIncorrectFormat);

            RuleFor(x => x.FullName).Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithName("Họ và tên").WithMessage(AppMessages.EMPTY_FULL_NAME);

            RuleFor(x => x.FullName).Cascade(CascadeMode.Stop)
                .Matches(LogicString.Validator.NameRegex)
                .WithName("Họ và tên").WithMessage(LogicString.Validator.NameIncorrectFormat);

            RuleFor(x => x.DateOfBirth).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithName("Ngày sinh").WithMessage(AppMessages.EMPTY_FULL_NAME);

            RuleFor(x => x.DateOfBirth).Cascade(CascadeMode.Stop)
                 .LessThan(DateOnly.FromDateTime(DateTime.Now.AddYears(-18)))
                .WithName("Ngày sinh").WithMessage(LogicString.Validator.UserUnder18);

            RuleFor(x => x.Gender).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithName("Giới tính").WithMessage(LogicString.Common.EmptyField);
        }
    }
}
