using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration
{
    internal sealed class AspNetRoleEntityTypeConfiguration : IEntityTypeConfiguration<AspNetRole>
    {
        public void Configure(EntityTypeBuilder<AspNetRole> builder)
        {
            builder.Property(r => r.Name).HasMaxLength(256);
            builder.Property(r => r.NormalizedName).HasMaxLength(256);

            builder.HasIndex(r => r.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");
        }
    }
}
