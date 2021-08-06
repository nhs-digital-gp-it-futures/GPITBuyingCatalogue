using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration
{
    internal sealed class AspNetUserClaimEntityTypeConfiguration : IEntityTypeConfiguration<AspNetUserClaim>
    {
        public void Configure(EntityTypeBuilder<AspNetUserClaim> builder)
        {
            builder.ToTable("AspNetUserClaims", Schemas.Users);

            builder.HasKey(uc => uc.Id);

            builder.Property(uc => uc.UserId).IsRequired();

            builder.HasOne(uc => uc.User)
                .WithMany(u => u.AspNetUserClaims)
                .HasForeignKey(uc => uc.UserId);

            builder.HasIndex(uc => uc.UserId, "IX_AspNetUserClaims_UserId");
        }
    }
}
