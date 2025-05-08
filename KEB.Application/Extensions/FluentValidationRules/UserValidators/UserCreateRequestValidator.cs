using FluentValidation;
using KEB.Application.DTOs.UserDTO;
using KEB.Domain.ValueObject;

namespace FSAKEB.Application.Extensions.FluentValidationRules
{
    public class UserCreateRequestValidator : AbstractValidator<UserCreateDTO>
    {
        public UserCreateRequestValidator()
        {
            RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
                .Matches(LogicString.Validator.EmailRegex)
                .WithName("Email").WithMessage(AppMessages.INVALID_EMAIL);
           
            RuleFor(x => x.FullName).Cascade(CascadeMode.Stop)
               .Matches(LogicString.Validator.NameRegex)
               .WithName("Họ và tên").WithMessage(AppMessages.INVALID_FULL_NAME);


            RuleFor(x => x.DateOfBirth).Cascade(CascadeMode.Stop)
                .NotEmpty().NotNull()
                .WithName("Ngày sinh").WithMessage(LogicString.Common.EmptyField);

            RuleFor(x => x.DateOfBirth).Cascade(CascadeMode.Stop)
                .LessThan(DateTime.Now.AddYears(-18))
                .WithName("Ngày sinh").WithMessage(LogicString.Validator.UserUnder18);

            //RuleFor(x => x.true).Cascade(CascadeMode.Stop)
            //    .NotEmpty().NotNull()
            //    .WithName("Giới tính").WithMessage(LogicString.Common.EmptyField);
        }
    }
}
