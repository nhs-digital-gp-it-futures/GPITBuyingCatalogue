using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CatalogueItemCapabilityStatusEntityTypeConfiguration : IEntityTypeConfiguration<CatalogueItemCapabilityStatus>
    {
        public void Configure(EntityTypeBuilder<CatalogueItemCapabilityStatus> builder)
        {
            builder.ToTable("CatalogueItemCapabilityStatus");

            builder.Property(s => s.Id).ValueGeneratedNever();
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(16);

            builder.HasIndex(s => s.Name, "IX_SolutionCapabilityStatusName")
                .IsUnique();
        }
    }
}
