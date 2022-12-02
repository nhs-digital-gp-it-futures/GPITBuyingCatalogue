using Microsoft.EntityFrameworkCore;
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

            builder.Property(c => c.Id).UseIdentityColumn();

            builder.Property(c => c.SupplierId).IsRequired();

            builder.Property(c => c.FirstName).HasMaxLength(35);
            builder.Property(c => c.LastName).HasMaxLength(35);
            builder.Property(c => c.Department).HasMaxLength(50);
            builder.Property(c => c.PhoneNumber).HasMaxLength(35);
            builder.Property(c => c.Email).HasMaxLength(255);

            builder.HasOne<Supplier>()
                .WithMany(s => s.SupplierContacts)
                .HasForeignKey(c => c.SupplierId)
                .HasConstraintName("FK_SupplierContacts_Supplier");

            builder.HasOne(s => s.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(s => s.LastUpdatedBy)
                .HasConstraintName("FK_SupplierContacts_LastUpdatedBy");

            builder.HasIndex(c => c.SupplierId, "IX_SupplierContacts_SupplierId");
        }
    }
}
