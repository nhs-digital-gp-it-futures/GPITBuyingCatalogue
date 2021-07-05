using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Supplier");

            builder.Property(s => s.Id).HasMaxLength(6);
            builder.Property(s => s.Address)
                .HasMaxLength(500)
                .HasConversion(
                    a => JsonSerializer.Serialize(a, null),
                    a => JsonSerializer.Deserialize<Address>(a, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

            builder.Property(s => s.LegalName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(s => s.OdsCode).HasMaxLength(8);
            builder.Property(s => s.Summary).HasMaxLength(1100);
            builder.Property(s => s.SupplierUrl).HasMaxLength(1000);

            builder.HasIndex(s => s.Name, "IX_SupplierName");
        }
    }
}
