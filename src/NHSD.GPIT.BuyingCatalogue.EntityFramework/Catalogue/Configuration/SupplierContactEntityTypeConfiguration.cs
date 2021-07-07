using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SupplierContactEntityTypeConfiguration : IEntityTypeConfiguration<SupplierContact>
    {
        public void Configure(EntityTypeBuilder<SupplierContact> builder)
        {
            builder.ToTable("SupplierContact");

            builder.HasKey(c => c.Id)
                .IsClustered(false);

            builder.HasIndex(c => c.SupplierId, "IX_SupplierContactSupplierId")
                .IsClustered();

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
