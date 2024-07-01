using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using PurpleIK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PurpleIK.Mapping
{
    public class CompanyMembershipMapping : IEntityTypeConfiguration<CompanyMembership>
    {
        public void Configure(EntityTypeBuilder<CompanyMembership> builder)
        {
            builder.Property(x => x.AutoId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
