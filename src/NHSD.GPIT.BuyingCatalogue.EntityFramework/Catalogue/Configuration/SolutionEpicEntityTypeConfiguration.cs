using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SolutionEpicEntityTypeConfiguration : IEntityTypeConfiguration<SolutionEpic>
    {
        public void Configure(EntityTypeBuilder<SolutionEpic> builder)
        {
            builder.ToTable("SolutionEpic");

            builder.HasKey(se => new { se.SolutionId, se.CapabilityId, se.EpicId });

            builder.Property(se => se.SolutionId)
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
                .HasForeignKey(se => se.SolutionId)
                .HasConstraintName("FK_SolutionEpic_Solution");

            builder.HasOne(se => se.Status)
                .WithMany()
                .HasForeignKey(se => se.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolutionEpicStatus");
        }
    }
}
