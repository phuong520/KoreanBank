using KEB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Configurations
{
    public class AddQuestionTaskConfiguration : IEntityTypeConfiguration<AddQuestionTask>
    {
        public void Configure(EntityTypeBuilder<AddQuestionTask> builder)
        {
            builder.ToTable(nameof(AddQuestionTask));
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.QuestionType).WithMany(r=>r.AddQuestions).HasForeignKey(x => x.QuestionTypeId);
            builder.HasOne(x => x.LevelDetail).WithMany(r=>r.AddQuestions).HasForeignKey(x => x.LevelDetailId);
            builder.HasOne(x => x.User).WithMany(r=>r.AddQuestions).HasForeignKey(x => x.AssignId);
        }
    }
}
