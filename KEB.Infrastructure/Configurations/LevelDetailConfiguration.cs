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
    public class LevelDetailConfiguration : IEntityTypeConfiguration<LevelDetail>
    {
        public void Configure(EntityTypeBuilder<LevelDetail> builder)
        {
            builder.ToTable(nameof(LevelDetail));
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Level).WithMany(r=>r.LevelDetails).HasForeignKey(x => x.LevelId);
            builder.HasOne(x=>x.Topic).WithMany(r=>r.LevelDetails).HasForeignKey(x=>x.TopicId);
        }
    }
}
