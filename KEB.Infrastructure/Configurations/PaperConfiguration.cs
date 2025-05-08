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
    public class PaperConfiguration : IEntityTypeConfiguration<Paper>
    {
        public void Configure(EntityTypeBuilder<Paper> builder)
        {
            builder.ToTable(nameof(Paper));
            builder.HasKey(p => p.Id);
            builder.HasOne(x=>x.Exam).WithMany(r=>r.Papers).HasForeignKey(x=>x.ExamId);
        }
    }
}
