using FluentValidation;
using KEB.Application.DTOs.QuestionAddDTO;
using KEB.Domain.ValueObject;

namespace FSAKEB.Application.Extensions.FluentValidationRules
{
    public class QuestionAddRequestValidator : AbstractValidator<AddSingleQuestionRequest>
    {
        public QuestionAddRequestValidator()
        {
            RuleFor(x => x.QuestionTypeId).Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithName("Dạng câu hỏi").WithMessage(LogicString.Common.EmptyField);
            RuleFor(x => x.RequestedUserId)
                .NotNull().WithMessage(LogicString.Common.CommitFailed);
            RuleFor(x => x.QuestionContent).Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithName("Nội dung câu hỏi").WithMessage(LogicString.Common.EmptyField);
        }
    }
}
