using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Configuration
{
    internal sealed class PublicationStatusEntityTypeConfiguration : IEntityTypeConfiguration<PublicationStatus>
    {
        public void Configure(EntityTypeBuilder<PublicationStatus> builder)
        {
            builder.ToTable("PublicationStatus");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(16);

            builder.HasIndex(e => e.Name, "IX_PublicationStatusName")
                .IsUnique();
        }
    }
}
