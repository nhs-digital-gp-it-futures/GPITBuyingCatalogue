using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Configuration
{
    internal sealed class TimeUnitEntityTypeConfiguration : IEntityTypeConfiguration<TimeUnit>
    {
        public void Configure(EntityTypeBuilder<TimeUnit> builder)
        {
            builder.ToTable("TimeUnit");

            builder.Property(e => e.TimeUnitId).ValueGeneratedNever();
            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}
