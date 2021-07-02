using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class FrameworkSolutionEntityTypeConfiguration : IEntityTypeConfiguration<FrameworkSolution>
    {
        public void Configure(EntityTypeBuilder<FrameworkSolution> builder)
        {
            builder.ToTable("FrameworkSolutions");

            builder.HasKey(f => new { f.FrameworkId, f.SolutionId });

            builder.Property(f => f.FrameworkId).HasMaxLength(10);
            builder.Property(f => f.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.HasOne(f => f.Framework)
                .WithMany()
                .HasForeignKey(f => f.FrameworkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FrameworkSolutions_Framework");

            builder.HasOne<Solution>()
                .WithMany(s => s.FrameworkSolutions)
                .HasForeignKey(f => f.SolutionId)
                .HasConstraintName("FK_FrameworkSolutions_Solution");
        }
    }
}
