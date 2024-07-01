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
    public class PersonMapping : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            // Tablo adı ve anahtar sütununu belirtme
            builder.ToTable("Person");
            builder.HasKey(p => p.Id);

            // Gender alanı için sütun özelliklerini belirtme
            builder.Property(p => p.Gender)
                .IsRequired(false); // Opsiyonel alan

            // ProfilePhoto alanı için sütun özelliklerini belirtme
            builder.Property(p => p.ProfilePhoto)
                .HasColumnType("varbinary(max)"); // Örnek veri türü

            // FirstName alanı için sütun özelliklerini belirtme
            builder.Property(p => p.FirstName)
                .HasMaxLength(50); // Örnek maksimum uzunluk

            // LastName alanı için sütun özelliklerini belirtme
            builder.Property(p => p.LastName)
                .HasMaxLength(50); // Örnek maksimum uzunluk

            // BirthDate alanı için sütun özelliklerini belirtme
            builder.Property(p => p.BirthDate)
                .IsRequired(false); // Opsiyonel alan

            // BirhtPlace alanı için sütun özelliklerini belirtme
            builder.Property(p => p.BirhtPlace)
                .HasMaxLength(100); // Örnek maksimum uzunluk

            // CitizenId alanı için sütun özelliklerini belirtme
            builder.Property(p => p.CitizenId)
                .HasMaxLength(20); // Örnek maksimum uzunluk

            // StartDate alanı için sütun özelliklerini belirtme
            builder.Property(p => p.StartDate)
                .IsRequired(false); // Opsiyonel alan

            // PersonalEmail alanı için sütun özelliklerini belirtme
            builder.Property(p => p.PersonalEmail)
                .HasMaxLength(100); // Örnek maksimum uzunluk
            builder.Property(p => p.CompanyEmail)
               .HasMaxLength(100); // Örnek maksimum uzunluk
            builder.Property(p => p.Password)
   .HasMaxLength(100); // Örnek maksimum uzunluk
            // Address alanı için sütun özelliklerini belirtme
            builder.Property(p => p.Address)
                .HasMaxLength(200); // Örnek maksimum uzunluk

            // PersonalPhoneNumber alanı için sütun özelliklerini belirtme
            builder.Property(p => p.PersonalPhoneNumber)
                .HasMaxLength(20); // Örnek maksimum uzunluk

            // Department alanı için sütun özelliklerini belirtme
            builder.Property(p => p.Department)
                .IsRequired(false); // Opsiyonel alan

            // NAV
            // CompanyId alanı için sütun özelliklerini belirtme
            builder.Property(p => p.CompanyId)
                .IsRequired(false); // Opsiyonel alan

            // AppUserId alanı için sütun özelliklerini belirtme
            builder.Property(p => p.AppUserId)
                .IsRequired(false); // Opsiyonel alan

            builder.Property(x => x.AutoId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
