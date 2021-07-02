using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CapabilityCategoryEntityTypeConfiguration : IEntityTypeConfiguration<CapabilityCategory>
    {
        public void Configure(EntityTypeBuilder<CapabilityCategory> builder)
        {
            builder.ToTable("CapabilityCategory");

            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(c => c.Name, "IX_CapabilityCategoryName")
                .IsUnique();
        }
    }
}
