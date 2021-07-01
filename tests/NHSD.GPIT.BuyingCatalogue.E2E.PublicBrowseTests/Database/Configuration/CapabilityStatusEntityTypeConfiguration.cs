using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Configuration
{
    internal sealed class CapabilityStatusEntityTypeConfiguration : IEntityTypeConfiguration<CapabilityStatus>
    {
        public void Configure(EntityTypeBuilder<CapabilityStatus> builder)
        {
            builder.ToTable("CapabilityStatus");

            builder.Property(e => e.Id).ValueGeneratedNever();
            builder.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(16);

            builder.HasIndex(e => e.Name, "IX_CapabilityStatusName")
                .IsUnique();
        }
    }
}
