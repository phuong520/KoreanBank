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
    public class ExamTypeConfiguration : IEntityTypeConfiguration<ExamType>
    {
        public void Configure(EntityTypeBuilder<ExamType> builder)
        {
            builder.ToTable(nameof(ExamTypeConfiguration));
            builder.HasKey(x => x.Id);
            builder.HasOne(e=>e.Levels).WithMany(r=>r.ExamTypes).HasForeignKey(e=>e.LevelId);
        }
    }
}
