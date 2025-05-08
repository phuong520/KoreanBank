using FluentValidation;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Domain.ValueObject;

namespace FSAKEB.Application.Extensions.FluentValidationRules
{
    public class AddQuestionTypeValidator : AbstractValidator<QuestionTypeCreateDto>
    {
        public AddQuestionTypeValidator()
        {
            RuleFor(x => x.QuestionTypeName).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithName("Dạng câu hỏi").WithMessage(LogicString.Common.EmptyField);
            RuleFor(x => x.QuestionTypeContent).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithName("Nội dung").WithMessage(LogicString.Common.EmptyField);
            RuleFor(x => x.Skill).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithName("Kỹ năng").WithMessage(LogicString.Common.EmptyField);
        }
    }
}
