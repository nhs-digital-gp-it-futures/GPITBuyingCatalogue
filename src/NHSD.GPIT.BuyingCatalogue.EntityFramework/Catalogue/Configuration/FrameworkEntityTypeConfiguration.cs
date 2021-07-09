using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class FrameworkEntityTypeConfiguration : IEntityTypeConfiguration<Framework>
    {
        public void Configure(EntityTypeBuilder<Framework> builder)
        {
            builder.ToTable("Frameworks", Schemas.Catalogue);

            builder.Property(f => f.Id).HasMaxLength(10);
            builder.Property(f => f.ActiveDate).HasColumnType("date");
            builder.Property(f => f.ExpiryDate).HasColumnType("date");
            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Owner).HasMaxLength(100);
            builder.Property(f => f.ShortName).HasMaxLength(25);
        }
    }
}
