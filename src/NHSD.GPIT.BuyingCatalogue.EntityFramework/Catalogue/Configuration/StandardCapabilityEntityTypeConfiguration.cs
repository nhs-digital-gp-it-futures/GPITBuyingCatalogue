using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class StandardCapabilityEntityTypeConfiguration : IEntityTypeConfiguration<StandardCapability>
    {
        public void Configure(EntityTypeBuilder<StandardCapability> builder)
        {
            builder.ToTable("StandardsCapabilities", Schemas.Catalogue);

            builder.HasKey(sc => new { sc.StandardId, sc.CapabilityId });

            builder.HasOne(sc => sc.Capability)
                .WithMany(c => c.StandardCapabilities)
                .HasForeignKey(sc => sc.CapabilityId)
                .HasConstraintName("FK_StandardsCapabilities_Capability")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sc => sc.Standard)
                .WithMany(s => s.StandardCapabilities)
                .HasForeignKey(sc => sc.StandardId)
                .HasConstraintName("FK_StandardsCapabilities_Standard")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
