using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration
{
    internal sealed class AspNetUserTokenEntityTypeConfiguration : IEntityTypeConfiguration<AspNetUserToken>
    {
        public void Configure(EntityTypeBuilder<AspNetUserToken> builder)
        {
            builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            builder.Property(t => t.LoginProvider).HasMaxLength(128);
            builder.Property(t => t.Name).HasMaxLength(128);

            builder.HasOne(t => t.User)
                .WithMany(u => u.AspNetUserTokens)
                .HasForeignKey(t => t.UserId);
        }
    }
}
