using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration
{
    internal sealed class AspNetRoleClaimEntityTypeConfiguration : IEntityTypeConfiguration<AspNetRoleClaim>
    {
        public void Configure(EntityTypeBuilder<AspNetRoleClaim> builder)
        {
            builder.HasIndex(rc => rc.RoleId, "IX_AspNetRoleClaims_RoleId");

            builder.Property(rc => rc.RoleId).IsRequired();

            builder.HasOne(rc => rc.Role)
                .WithMany(r => r.AspNetRoleClaims)
                .HasForeignKey(rc => rc.RoleId);
        }
    }
}
