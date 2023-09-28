using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class FeaturesCriteriaEntityTypeConfiguration : IEntityTypeConfiguration<FeaturesCriteria>
{
    public void Configure(EntityTypeBuilder<FeaturesCriteria> builder)
    {
        builder.ToTable("FeaturesCriteria", Schemas.Competitions);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Requirements).IsRequired().HasMaxLength(1100);

        builder.Property(x => x.Compliance)
            .HasConversion<int>();

        builder.HasOne<NonPriceElements>()
            .WithMany(x => x.Features)
            .HasForeignKey(x => x.NonPriceElementsId)
            .HasConstraintName("FK_FeaturesCriteria_NonPriceElements")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
