using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Configuration
{
    internal sealed class CompliancyLevelEntityTypeConfiguration : IEntityTypeConfiguration<CompliancyLevel>
    {
        public void Configure(EntityTypeBuilder<CompliancyLevel> builder)
        {
            builder.ToTable("CompliancyLevel");

            builder.Property(e => e.Id).ValueGeneratedNever();
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(16);

            builder.HasIndex(e => e.Name, "IX_CompliancyLevelName")
                .IsUnique();
        }
    }
}
