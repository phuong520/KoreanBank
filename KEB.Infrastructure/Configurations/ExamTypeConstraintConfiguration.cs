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
    public class ExamTypeConstraintConfiguration : IEntityTypeConfiguration<ExamTypeConstraint>
    {
        public void Configure(EntityTypeBuilder<ExamTypeConstraint> builder)
        {
            builder.ToTable(nameof(ExamTypeConstraint));
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.ExamType).WithMany(r=>r.ExamTypeConstraints).HasForeignKey(x=>x.ExamTypeId);
        }
    }
}
