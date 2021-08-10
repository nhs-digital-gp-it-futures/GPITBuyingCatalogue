using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Configuration
{
    internal sealed class CatalogueItemTypeEntityTypeConfiguration : IEntityTypeConfiguration<CatalogueItemType>
    {
        public void Configure(EntityTypeBuilder<CatalogueItemType> builder)
        {
            builder.ToTable("CatalogueItemType");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(e => e.Name, "AK_CatalogueItemType_Name")
                    .IsUnique();
        }
    }
}
