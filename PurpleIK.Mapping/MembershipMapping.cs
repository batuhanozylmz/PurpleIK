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
    public class MembershipMapping : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.ToTable("Memberships");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(50);
            builder.Property(m => m.SubscriptionPeriod).IsRequired(false).HasMaxLength(50);

            builder.Property(m => m.Price).IsRequired(false).HasColumnType("decimal(8,2)");
            builder.Property(m => m.Description).IsRequired(false).HasMaxLength(250);
            builder.Property(m => m.NumberOfEmployee).IsRequired(false);            
            builder.Property(x => x.AutoId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);          

        }
    }
}
