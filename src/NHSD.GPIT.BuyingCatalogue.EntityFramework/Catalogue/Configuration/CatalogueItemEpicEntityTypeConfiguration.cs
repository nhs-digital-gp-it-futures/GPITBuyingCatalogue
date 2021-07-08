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
            builder.ToTable("CatalogueItemEpic");

            builder.HasKey(se => new { se.CatalogueItemId, se.CapabilityId, se.EpicId });

            builder.Property(se => se.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(se => se.EpicId).HasMaxLength(10);
            builder.HasOne<Capability>()
                .WithMany()
                .HasForeignKey(d => d.CapabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolutionEpic_Capability");

            builder.HasOne(se => se.Epic)
                .WithMany()
                .HasForeignKey(se => se.EpicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolutionEpic_Epic");

            builder.HasOne<Solution>()
                .WithMany(s => s.SolutionEpics)
                .HasForeignKey(se => se.CatalogueItemId)
                .HasConstraintName("FK_SolutionEpic_Solution");

            builder.HasOne(se => se.Status)
                .WithMany()
                .HasForeignKey(se => se.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolutionEpic_Status");
        }
    }
}
