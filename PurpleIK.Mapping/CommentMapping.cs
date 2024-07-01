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
    public class CommentMapping : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            // Tablo adı ve anahtar sütununu belirtme
            builder.ToTable("Comments"); // "Comments" adında bir tablo oluşturulur

            builder.HasKey(c => c.Id); // Id alanı anahtar olarak belirlenir

            builder.Property(c => c.CommentText)
                   .HasMaxLength(2000) // Maksimum 500 karakter uzunluğunda
                   .IsRequired(); // Zorunlu alan

            builder.Property(c => c.Title)
                   .HasMaxLength(50).IsRequired(false); // Maksimum 50 karakter uzunluğunda

            builder.Property(c => c.Summary)
                   .HasMaxLength(200).IsRequired(false); // Maksimum 200 karakter uzunluğunda
                                        
            builder.Property(c => c.Photo).IsRequired(false); // Photo alanı nullable olduğu için özel bir kısıtlama eklenmez

            // Person alanı için ilişkisel özelliklerini belirtme
            builder.HasOne(c => c.Person).WithMany(c => c.Comments).HasForeignKey(c => c.PersonId).IsRequired(false);

            builder.Property(x => x.AutoId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
