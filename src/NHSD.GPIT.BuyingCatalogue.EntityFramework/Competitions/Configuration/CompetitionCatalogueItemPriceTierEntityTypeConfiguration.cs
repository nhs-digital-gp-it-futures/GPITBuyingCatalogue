using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public class CompetitionCatalogueItemPriceTierEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionCatalogueItemPriceTier>
{
    public void Configure(EntityTypeBuilder<CompetitionCatalogueItemPriceTier> builder)
    {
        builder.ToTable("CompetitionCatalogueItemPriceTiers", Schemas.Competitions);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompetitionId).IsRequired();

        builder.Property(x => x.CompetitionItemPriceId).IsRequired();

        builder.Property(x => x.Price)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.ListPrice)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.HasOne(x => x.CompetitionCatalogueItemPrice)
            .WithMany(x => x.Tiers)
            .HasForeignKey(x => x.CompetitionItemPriceId)
            .HasConstraintName("FK_CompetitionCatalogueItemPriceTiers_CompetitionItemPriceId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
