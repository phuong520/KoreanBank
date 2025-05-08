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
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.ToTable(nameof(Exam));
            builder.HasKey(x => x.Id);
            builder.HasOne(e=>e.ExamType).WithMany(r=>r.Exams).HasForeignKey(e=>e.ExamTypeId);
            builder.HasOne(x=>x.User).WithMany(r=>r.Exams).HasForeignKey(e=>e.HostId);
            builder.HasOne(x=>x.User).WithMany(r=>r.Exams).HasForeignKey(e=>e.ReviewerId);
        }
    }
}
