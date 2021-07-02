using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SolutionCapabilityEntityTypeConfiguration : IEntityTypeConfiguration<SolutionCapability>
    {
        public void Configure(EntityTypeBuilder<SolutionCapability> builder)
        {
            builder.HasKey(sc => new { sc.SolutionId, sc.CapabilityId });

            builder.ToTable("SolutionCapability");

            builder.Property(sc => sc.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.HasOne(sc => sc.Capability)
                .WithMany()
                .HasForeignKey(sc => sc.CapabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolutionCapability_Capability");

            builder.HasOne<Solution>()
                .WithMany(s => s.SolutionCapabilities)
                .HasForeignKey(sc => sc.SolutionId)
                .HasConstraintName("FK_SolutionCapability_Solution");

            builder.HasOne(sc => sc.Status)
                .WithMany()
                .HasForeignKey(sc => sc.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolutionCapability_SolutionCapabilityStatus");
        }
    }
}
