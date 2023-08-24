using System;
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

            builder.Property(s => s.StandardType)
                .IsRequired()
                .HasColumnName("StandardTypeId");

            builder.Property(s => s.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasQueryFilter(o => !o.IsDeleted);

            builder.HasMany(s => s.StandardCapabilities)
                .WithOne(sc => sc.Standard)
                .HasForeignKey(sc => sc.StandardId);

            builder.HasOne(s => s.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(s => s.LastUpdatedBy)
                .HasConstraintName("FK_Standards_LastUpdatedBy");
        }
    }
}
