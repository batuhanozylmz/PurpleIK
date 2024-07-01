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
    public class PersonalInformationMapping : IEntityTypeConfiguration<PersonalInformation>
    {
        public void Configure(EntityTypeBuilder<PersonalInformation> builder)
        {
            builder.ToTable("PersonalInformation");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FormName).HasMaxLength(50).IsRequired(false);

            builder.HasOne(p => p.Manager).WithMany(p => p.PersonalInformations).HasForeignKey(p => p.ManagerId).IsRequired(false);

            builder.Property(p => p.PersonalInformationForm).HasColumnType("varbinary(max)");

            builder.Property(p => p.AutoId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
