using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Configuration
{
    internal sealed class ProvisioningTypeEntityTypeConfiguration : IEntityTypeConfiguration<ProvisioningType>
    {
        public void Configure(EntityTypeBuilder<ProvisioningType> builder)
        {
            builder.ToTable("ProvisioningType");

            builder.Property(e => e.ProvisioningTypeId).ValueGeneratedNever();
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(35);
        }
    }
}
