using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterEntityTypeConfiguration : IEntityTypeConfiguration<Filter>
    {
        public void Configure(EntityTypeBuilder<Filter> builder)
        {
            builder.ToTable("Filters", Schemas.Filtering);

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.Description)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.OrganisationId)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(f => f.FrameworkId)
                .HasMaxLength(10);

            builder.Property(i => i.Created).HasDefaultValue(DateTime.UtcNow);

            builder.Property(i => i.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(i => i.Organisation)
                .WithMany()
                .HasForeignKey(i => i.OrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Filters_OrganisationId");

            builder.HasOne(i => i.Framework)
                .WithMany()
                .HasForeignKey(i => i.FrameworkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Filters_FrameworkId");

            builder.HasOne(i => i.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(i => i.LastUpdatedBy)
                .HasConstraintName("FK_Filters_LastUpdatedBy");
        }
    }
}
