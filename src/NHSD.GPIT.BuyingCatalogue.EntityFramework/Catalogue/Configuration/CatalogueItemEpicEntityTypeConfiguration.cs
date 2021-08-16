using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CatalogueItemEpicEntityTypeConfiguration : IEntityTypeConfiguration<CatalogueItemEpic>
    {
        public void Configure(EntityTypeBuilder<CatalogueItemEpic> builder)
        {
            builder.ToTable("CatalogueItemEpics", Schemas.Catalogue);

            builder.HasKey(se => new { se.CatalogueItemId, se.CapabilityId, se.EpicId });

            builder.Property(e => e.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(e => e.EpicId).HasMaxLength(10);
            builder.HasOne<Capability>()
                .WithMany()
                .HasForeignKey(e => e.CapabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatalogueItemEpics_Capability");

            builder.HasOne(e => e.Epic)
                .WithMany()
                .HasForeignKey(e => e.EpicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatalogueItemEpics_Epic");

            builder.HasOne<CatalogueItem>()
                .WithMany(i => i.CatalogueItemEpics)
                .HasForeignKey(e => e.CatalogueItemId)
                .HasConstraintName("FK_CatalogueItemEpics_CatalogueItem");

            builder.HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatalogueItemEpics_Status");

            builder.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedBy)
                .HasConstraintName("FK_CatalogueItemEpics_LastUpdatedBy");
        }
    }
}
