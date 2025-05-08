using KEB.Application.DTOs.AnswerDTO;
using KEB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.QuestionAddDTO
{
    public class ProcessedQuestionDTO
    {
        public Guid CreatedUserId { get; set; }
        public string LevelNameAndTopicName { get; set; }
        public string ReferenceName { get; set; }
        public int ReferencePublishedYear { get; set; }
        public string QuestionTypeName { get; set; } = string.Empty;
        public Skill Skill { get; set; }
        public Difficulty Difficulty { get; set; }
        public string QuestionContent { get; set; } = string.Empty;
        public List<AnswerDTO.AddAnswerDTO> Answers { get; set; } = [];
        public string AttachmentFileName { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is ProcessedQuestionDTO other)
            {
                // So sánh nội dung câu hỏi và câu trả lời
                bool answerDup = true;
                foreach (var answer in other.Answers)
                {
                    if (!Answers.Contains(answer))
                    {
                        answerDup = false;
                        break;
                    }
                }
                return QuestionContent.Equals(other.QuestionContent) && answerDup;
                //&& CompareUtils.Are2ListAnswersSimilar(Answers, other.Answers);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(QuestionContent, QuestionTypeName);
        }

    }

}
