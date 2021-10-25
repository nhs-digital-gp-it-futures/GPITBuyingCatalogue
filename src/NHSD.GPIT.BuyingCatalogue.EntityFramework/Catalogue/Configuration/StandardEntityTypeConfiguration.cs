using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class StandardEntityTypeConfiguration : IEntityTypeConfiguration<Standard>
    {
        public void Configure(EntityTypeBuilder<Standard> builder)
        {
            builder.ToTable("Standards", Schemas.Catalogue);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .HasMaxLength(5)
                .ValueGeneratedNever();

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.Url)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(s => s.RequiredForAllSolutions)
                .IsRequired();

            builder.HasMany(s => s.StandardCapabilities)
                .WithOne(sc => sc.Standard)
                .HasForeignKey(sc => sc.StandardId);
        }
    }
}
