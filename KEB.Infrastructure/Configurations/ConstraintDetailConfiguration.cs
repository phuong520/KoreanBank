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
    public class ConstraintDetailConfiguration : IEntityTypeConfiguration<ConstraintDetail>
    {
        public void Configure(EntityTypeBuilder<ConstraintDetail> builder)
        {
            builder.ToTable(nameof(ConstraintDetail));
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.Topic).WithMany(r=>r.ConstraintDetails).HasForeignKey(x=>x.TopicId);
            builder.HasOne(x => x.QuestionType).WithMany(r => r.ConstraintDetails).HasForeignKey(x => x.QuestionTypeId);
            builder.HasOne(x=>x.ExamTypeConstraint).WithMany(r=>r.ConstraintDetails).HasForeignKey(x => x.ExamTypeConstraintId);
        }
    }
}
