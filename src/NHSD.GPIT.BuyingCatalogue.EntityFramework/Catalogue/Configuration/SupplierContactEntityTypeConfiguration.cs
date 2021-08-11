﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SupplierContactEntityTypeConfiguration : IEntityTypeConfiguration<SupplierContact>
    {
        public void Configure(EntityTypeBuilder<SupplierContact> builder)
        {
            builder.ToTable("SupplierContacts", Schemas.Catalogue);

            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.SupplierId, "IX_SupplierContactSupplierId");

            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(35);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(35);

            builder.Property(c => c.PhoneNumber).HasMaxLength(35);
            builder.Property(c => c.SupplierId)
                .IsRequired()
                .HasMaxLength(6);

            builder.HasOne<Supplier>()
                .WithMany(s => s.SupplierContacts)
                .HasForeignKey(c => c.SupplierId)
                .HasConstraintName("FK_SupplierContact_Supplier");
        }
    }
}
