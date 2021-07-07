using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration
{
    internal sealed class AspNetUserRoleEntityTypeConfiguration : IEntityTypeConfiguration<AspNetUserRole>
    {
        public void Configure(EntityTypeBuilder<AspNetUserRole> builder)
        {
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.HasIndex(ur => ur.RoleId, "IX_AspNetUserRoles_RoleId");

            builder.HasOne(ur => ur.Role)
                .WithMany(r => r.AspNetUserRoles)
                .HasForeignKey(ur => ur.RoleId);

            builder.HasOne(ur => ur.User)
                .WithMany(u => u.AspNetUserRoles)
                .HasForeignKey(ur => ur.UserId);
        }
    }
}
