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
    public class CompanyMapping : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            // Tablo adı ve anahtar sütununu belirtme
            builder.ToTable("Companies");
            builder.HasKey(c => c.Id);

            // CompanyName alanı için sütun özelliklerini belirtme
            builder.Property(c => c.CompanyName)
                .HasMaxLength(100) // Örnek maksimum uzunluk
                .IsRequired(); // Zorunlu alan

            // MersisNo alanı için sütun özelliklerini belirtme
            builder.Property(c => c.MersisNo)
                .HasMaxLength(20); // Örnek maksimum uzunluk

            // TaxNo alanı için sütun özelliklerini belirtme
            builder.Property(c => c.TaxNo)
                .HasMaxLength(20); // Örnek maksimum uzunluk

            // Logo alanı için sütun özelliklerini belirtme
            builder.Property(c => c.Logo)
                .HasColumnType("varbinary(max)"); // Örnek veri türü

            // PhoneNumber alanı için sütun özelliklerini belirtme
            builder.Property(c => c.PhoneNumber)
                .HasMaxLength(20); // Örnek maksimum uzunluk

            // Address alanı için sütun özelliklerini belirtme
            builder.Property(c => c.Address)
                .HasMaxLength(200); // Örnek maksimum uzunluk

            // Email alanı için sütun özelliklerini belirtme
            builder.Property(c => c.CompanyEmail)
                .HasMaxLength(100); // Örnek maksimum uzunluk     
            builder.Property(c => c.SuperManagerPassword)
               .HasMaxLength(100); // Örnek maksimum uzunluk
            builder.Property(c => c.SuperManagerEmail)
               .HasMaxLength(100); // Örnek maksimum uzunluk  
            builder.Property(c => c.SuperManagerFirstName)
                 .HasMaxLength(100); // Örnek maksimum uzunluk  
            builder.Property(c => c.SuperManagerLastName)
               .HasMaxLength(100); // Örnek maksimum uzunluk  
            // CompanyTypes alanı için ilişkisel özelliklerini belirtme
            builder.Property(c => c.CompanyTypes)
            .IsRequired(false); // Opsiyonel alan

            // NumberOfEmployees alanı için sütun özelliklerini belirtme
            builder.Property(c => c.NumberOfEmployees)
                .IsRequired(false); // Opsiyonel alan


            builder.Property(x => x.AutoId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
