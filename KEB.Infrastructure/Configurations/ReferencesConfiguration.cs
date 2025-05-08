using KEB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Infrastructure.Configurations
{
    public class ReferencesConfiguration : IEntityTypeConfiguration<References>
    {
        public void Configure(EntityTypeBuilder<References> builder)
        {
            builder.ToTable(nameof(References));
            builder.HasKey(x => x.Id);
        }
    }
}
