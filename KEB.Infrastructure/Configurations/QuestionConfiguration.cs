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
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable(nameof(Question));
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.References).WithMany(r=>r.Questions).HasForeignKey(x=>x.ReferenceId);
            builder.HasOne(x=>x.QuestionType).WithMany(r=>r.Questions).HasForeignKey(x=>x.QuestionTypeId);
            builder.HasOne(x=>x.LevelDetail).WithMany(r=>r.Questions).HasForeignKey(x=>x.LevelDetailId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x=>x.SystemAccessLog).WithMany(r=>r.Questions).HasForeignKey(x=>x.LogId);
            builder.HasOne(x=>x.AddQuestionTask).WithMany(r=>r.Questions).HasForeignKey(x=>x.TaskId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.AttachmentFileImage).WithOne(r => r.QuestionForImage).HasForeignKey<Question>(x=>x.AttachFileImageId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.AttachmentFileAudio).WithOne(r => r.QuestionForAudio).HasForeignKey<Question>(x=>x.AttachFileAudioId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
