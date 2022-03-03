using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class ContactEntityTypeConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contacts", Schemas.Ordering);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.Email).HasMaxLength(256);
            builder.Property(c => c.FirstName).HasMaxLength(100);
            builder.Property(c => c.LastName).HasMaxLength(100);
            builder.Property(c => c.Phone).HasMaxLength(35);
            builder.Property(c => c.Department).HasMaxLength(50);
            builder.Property(c => c.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(c => c.SupplierContact)
                .WithMany()
                .HasForeignKey(c => c.SupplierContactId)
                .HasConstraintName("FK_Contacts_SupplierContactId");

            builder.HasOne(c => c.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(c => c.LastUpdatedBy)
                .HasConstraintName("FK_Contacts_LastUpdatedBy");
        }
    }
}
