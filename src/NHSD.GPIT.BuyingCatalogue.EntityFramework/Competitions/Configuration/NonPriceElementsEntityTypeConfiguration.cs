using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class NonPriceElementsEntityTypeConfiguration : IEntityTypeConfiguration<NonPriceElements>
{
    public void Configure(EntityTypeBuilder<NonPriceElements> builder)
    {
        builder.ToTable("NonPriceElements", Schemas.Competitions);
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Competition)
            .WithOne(x => x.NonPriceElements)
            .HasForeignKey<NonPriceElements>(x => x.CompetitionId)
            .HasConstraintName("FK_NonPriceElements_Competition")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Implementation)
            .WithOne()
            .HasForeignKey<NonPriceElements>(x => x.ImplementationId)
            .HasConstraintName("FK_NonPriceElements_ImplementationCriteria")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ServiceLevel)
            .WithOne()
            .HasForeignKey<NonPriceElements>(x => x.ServiceLevelId)
            .HasConstraintName("FK_NonPriceElements_ServiceLevelCriteria")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Interoperability)
            .WithOne()
            .HasForeignKey(x => x.NonPriceElementsId)
            .HasConstraintName("FK_InteroperabilityCriteria_NonPriceElements")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
