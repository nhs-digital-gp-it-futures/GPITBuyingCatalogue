using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CatalogueItemEpicStatusEntityTypeConfiguration : IEntityTypeConfiguration<CatalogueItemEpicStatus>
    {
        public void Configure(EntityTypeBuilder<CatalogueItemEpicStatus> builder)
        {
            builder.ToTable("CatalogueItemEpicStatus", Schemas.Catalogue);

            builder.Property(s => s.Id).ValueGeneratedNever();
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(16);

            builder.HasIndex(s => s.Name, "IX_EpicStatusName")
                .IsUnique();
        }
    }
}
