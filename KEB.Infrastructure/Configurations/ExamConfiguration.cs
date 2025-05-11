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
            builder.HasOne(x=>x.Host).WithMany(r=>r.HostedExams).HasForeignKey(e=>e.HostId).OnDelete(DeleteBehavior.Restrict); ;
            builder.HasOne(x=>x.Reviewer).WithMany(r=>r.ReviewedExams).HasForeignKey(e=>e.ReviewerId).OnDelete(DeleteBehavior.Restrict); ;
        }
    }
}
