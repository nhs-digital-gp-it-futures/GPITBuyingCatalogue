using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Configuration;

public sealed class CompetitionCatalogueItemEntityTypeConfiguration : IEntityTypeConfiguration<CompetitionCatalogueItem>
{
    public void Configure(EntityTypeBuilder<CompetitionCatalogueItem> builder)
    {
        builder.UseTphMappingStrategy();

        builder.ToTable("CompetitionCatalogueItems", Schemas.Competitions);

        builder.HasQueryFilter(x => !(x is CompetitionSolution) || ((CompetitionSolution)x).IsShortlisted);

        builder.HasKey(x => x.Id);

        builder.HasDiscriminator(x => x.CatalogueItemType)
            .HasValue<CompetitionSolution>(CatalogueItemType.Solution)
            .HasValue<CompetitionAdditionalService>(CatalogueItemType.AdditionalService)
            .HasValue<CompetitionAssociatedService>(CatalogueItemType.AssociatedService);

        builder.Property(x => x.CatalogueItemType).HasConversion<int>();

        builder.HasOne(x => x.CatalogueItem)
            .WithMany()
            .HasForeignKey(x => x.CatalogueItemId)
            .HasConstraintName("FK_CompetitionCatalogueItems_CatalogueItem");

        builder.HasOne(x => x.Price)
            .WithOne()
            .HasForeignKey<CompetitionCatalogueItemPrice>(x => x.CompetitionCatalogueItemId)
            .HasConstraintName("FK_CompetitionCatalogueItemPrice_CatalogueItem")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Quantities)
            .WithOne()
            .HasForeignKey(x => x.CompetitionItemId)
            .HasConstraintName("FK_CompetitionItemQuantities_CompetitionItem")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Services)
            .WithOne()
            .HasForeignKey(x => x.ParentItemId)
            .HasConstraintName("FK_CompetitionCatalogueItems_Parent")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
