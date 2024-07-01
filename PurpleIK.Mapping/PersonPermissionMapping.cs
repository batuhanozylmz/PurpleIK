using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using PurpleIK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleIK.Mapping
{
    public class PersonPermissionMapping : IEntityTypeConfiguration<PersonPermission>
    {
        public void Configure(EntityTypeBuilder<PersonPermission> builder)
        {
            builder.ToTable("PersonelPermissions");

            builder.HasKey(pp => pp.Id);

            builder.Property(pp => pp.StartDate).HasColumnName("StartDate");
            builder.Property(pp => pp.EndDate).HasColumnName("EndDate");
            builder.Property(pp => pp.NumberOfDays).HasColumnName("NumberOfDays");
            builder.Property(pp => pp.DateOfReply).HasColumnName("DateOfReply");
            builder.Property(pp => pp.PermissionFile).HasColumnName("PermissionFile");
            builder.Property(pp => pp.ReasonOfRejection).HasColumnName("ReasonOfRejection");
            builder.Property(pp => pp.CompanyManagerName).HasColumnName("CompanyManagerName");
            builder.Property(pp => pp.CompanyManagerEmail).HasColumnName("CompanyManagerEmail");
            builder.Property(pp => pp.PersonId).HasColumnName("PersonId");
            builder.Property(pp => pp.PermissionId).HasColumnName("PermissionId");

            builder.HasOne(pp => pp.Person)
                   .WithMany(p => p.PersonPermissions)
                   .HasForeignKey(pp => pp.PersonId);

            builder.HasOne(pp => pp.Permission)
                   .WithMany()
                   .HasForeignKey(pp => pp.PermissionId);

            builder.Property(x => x.AutoId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}