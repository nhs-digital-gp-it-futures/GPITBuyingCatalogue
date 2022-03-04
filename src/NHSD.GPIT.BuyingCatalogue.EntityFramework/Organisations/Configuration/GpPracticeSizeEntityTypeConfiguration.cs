using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Configuration
{
    internal sealed class GpPracticeSizeEntityTypeConfiguration : IEntityTypeConfiguration<GpPracticeSize>
    {
        public void Configure(EntityTypeBuilder<GpPracticeSize> builder)
        {
            builder.ToTable("GpPracticeSize", Schemas.Organisations);

            builder.HasKey(g => g.OdsCode);

            builder.Property(g => g.OdsCode).HasMaxLength(10);
            builder.Property(g => g.NumberOfPatients).IsRequired();
            builder.Property(g => g.ExtractDate).IsRequired();
        }
    }
}
