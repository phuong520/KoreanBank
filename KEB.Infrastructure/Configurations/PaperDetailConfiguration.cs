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
            builder.HasOne(x => x.AttachmentImage).WithOne(f => f.PaperDetailImage).HasForeignKey<PaperDetail>(x => x.AttachmentImageId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.AttachmentAudio).WithOne(aud => aud.PaperDetailAudio).HasForeignKey<PaperDetail>(x => x.AttachmentAudioId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
