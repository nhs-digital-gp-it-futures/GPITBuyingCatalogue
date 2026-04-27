using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public class SolutionStandardEntityTypeConfiguration : IEntityTypeConfiguration<SolutionStandard>
{
    public void Configure(EntityTypeBuilder<SolutionStandard> builder)
    {
        builder.ToTable("SolutionStandards", Schemas.Catalogue);

        builder.HasKey(ss => new { ss.SolutionId, ss.StandardId });

        builder.Property(ss => ss.StandardId).HasMaxLength(5);
        builder.Property(ss => ss.SolutionId).HasMaxLength(14);

        builder.HasOne(x => x.Standard).WithMany().HasForeignKey(ss => ss.StandardId);
        builder.HasOne<Solution>().WithMany(x => x.SolutionStandards).HasForeignKey(ss => ss.SolutionId);
    }
}
