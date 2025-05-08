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
    public class SystemAccessLogConfiguration : IEntityTypeConfiguration<SystemAccessLog>
    {
        public void Configure(EntityTypeBuilder<SystemAccessLog> builder)
        {
            builder.ToTable(nameof(SystemAccessLog));
            builder.HasKey(x => x.Id);
            builder.HasOne(x=>x.User).WithMany(r=>r.SystemAccessLogs).HasForeignKey(x=>x.UserId);
        }
    }
}
