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
    public class PaperDetailConfiguration : IEntityTypeConfiguration<PaperDetail>
    {
        public void Configure(EntityTypeBuilder<PaperDetail> builder)
        {
            builder.ToTable(nameof(PaperDetail));
            builder.HasKey(pd => new { pd.PaperId, pd.QuestionId });
            builder.HasOne(x=>x.Paper).WithMany(r=>r.PaperDetails).HasForeignKey(x=>x.PaperId);
            builder.HasOne(x=>x.Question).WithMany(r=>r.PaperDetails).HasForeignKey(x=>x.QuestionId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Attachment).WithOne(r => r.PaperDetail).HasForeignKey<PaperDetail>(x => x.AttachmentId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
