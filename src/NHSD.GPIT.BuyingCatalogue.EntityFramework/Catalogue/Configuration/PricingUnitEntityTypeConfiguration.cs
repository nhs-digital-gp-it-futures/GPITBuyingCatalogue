using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class PricingUnitEntityTypeConfiguration : IEntityTypeConfiguration<PricingUnit>
    {
        public void Configure(EntityTypeBuilder<PricingUnit> builder)
        {
            builder.ToTable("PricingUnit");

            builder.HasKey(u => u.PricingUnitId)
                .IsClustered(false);

            builder.Property(u => u.PricingUnitId).ValueGeneratedNever();
            builder.Property(u => u.Description)
                .IsRequired()
                .HasMaxLength(40);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.TierName)
                .IsRequired()
                .HasMaxLength(30);
        }
    }
}
