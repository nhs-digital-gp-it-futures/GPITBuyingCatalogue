using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SolutionEntityTypeConfiguration : IEntityTypeConfiguration<Solution>
    {
        public void Configure(EntityTypeBuilder<Solution> builder)
        {
            builder.ToTable("Solutions", Schemas.Catalogue);

            builder.HasKey(s => s.CatalogueItemId);

            builder.Property(s => s.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(s => s.AboutUrl).HasMaxLength(1000);
            builder.Property(s => s.FullDescription).HasMaxLength(3000);
            builder.Property(s => s.ImplementationDetail).HasMaxLength(1100);
            builder.Property(s => s.IntegrationsUrl).HasMaxLength(1000);
            builder.Property(s => s.IsPilotSolution).HasDefaultValue(false);
            builder.Property(s => s.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var converter = new ValueConverter<ClientApplication, string>(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions)null),
                serializedValue => JsonSerializer.Deserialize<ClientApplication>(serializedValue, jsonSerializerOptions));

            var comparer = new ValueComparer<ClientApplication>(
                (left, right) => JsonSerializer.Serialize(left, (JsonSerializerOptions)null) == JsonSerializer.Serialize(right, (JsonSerializerOptions)null),
                value => value == null ? 0 : JsonSerializer.Serialize(value, (JsonSerializerOptions)null).GetHashCode(),
                value => JsonSerializer.Deserialize<ClientApplication>(JsonSerializer.Serialize(value, (JsonSerializerOptions)null), jsonSerializerOptions));

            builder.Property(s => s.ClientApplication)
                .HasConversion(converter, comparer);

            builder.OwnsOne(s => s.Hosting, h =>
            {
                h.OwnsOne(c => c.HybridHostingType);
                h.OwnsOne(c => c.PublicCloud);
                h.OwnsOne(c => c.PrivateCloud);
                h.OwnsOne(c => c.OnPremise);
                h.ToJson();
            });

            builder.Property(s => s.ApplicationType)
                .HasColumnName("ClientApplication");

            builder.Property(s => s.RoadMap).HasMaxLength(1000);
            builder.Property(s => s.Summary).HasMaxLength(350);

            builder.HasOne(s => s.CatalogueItem)
                .WithOne(i => i.Solution)
                .HasForeignKey<Solution>(s => s.CatalogueItemId)
                .HasConstraintName("FK_Solutions_CatalogueItem");

            builder.HasOne(s => s.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(s => s.LastUpdatedBy)
                .HasConstraintName("FK_Solutions_LastUpdatedBy");
        }
    }
}
