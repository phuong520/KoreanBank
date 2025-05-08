using FluentValidation;
using KEB.Application.DTOs.QuestionTypeDTO;
using KEB.Domain.ValueObject;

namespace FSAKEB.Application.Extensions.FluentValidationRules.QuestionTypeValidators
{
    public class UpdateQuestionTypeValidator : AbstractValidator<QuestionTypeUpdateDto>
    {
        public UpdateQuestionTypeValidator()
        {
            RuleFor(x => x.QuestionTypeId).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithName("Id của loại câu hỏi").WithMessage(LogicString.Common.EmptyField);
            RuleFor(x => x.QuestionTypeName).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithName("Dạng câu hỏi").WithMessage(LogicString.Common.EmptyField);
            RuleFor(x => x.QuestionTypeContent).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithName("Nội dung").WithMessage(LogicString.Common.EmptyField);
        }
    }
}
