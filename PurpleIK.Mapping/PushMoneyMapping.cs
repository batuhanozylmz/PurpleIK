using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PurpleIK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Mapping
{
    public class PushMoneyMapping : IEntityTypeConfiguration<PushMoney>
    {
        public void Configure(EntityTypeBuilder<PushMoney> builder)
        {
            builder.ToTable("PushMoney");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(10,2)");
            builder.Property(p => p.Type).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(2500).IsRequired(false);
            builder.Property(p => p.CurrencyUnit).HasMaxLength(10).IsRequired(false);
            builder.HasOne(p => p.Person).WithMany(p => p.PushMoneys).HasForeignKey(p => p.PersonId).IsRequired(false);
            builder.Property(p => p.AutoId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
