using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Configuration
{
    internal sealed class AspNetRoleEntityTypeConfiguration : IEntityTypeConfiguration<AspNetRole>
    {
        public void Configure(EntityTypeBuilder<AspNetRole> builder)
        {
            builder.ToTable("AspNetRoles", Schemas.Users);

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            builder.Property(r => r.Name).HasMaxLength(256).IsRequired();
            builder.Property(r => r.NormalizedName).HasMaxLength(256).IsRequired();

            builder.HasIndex(r => r.NormalizedName, "AK_AspNetRoles_NormalizedName")
                .IsUnique();
        }
    }
}
