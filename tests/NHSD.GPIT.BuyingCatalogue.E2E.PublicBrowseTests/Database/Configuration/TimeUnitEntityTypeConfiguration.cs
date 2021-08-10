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

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).ValueGeneratedNever();
            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}
