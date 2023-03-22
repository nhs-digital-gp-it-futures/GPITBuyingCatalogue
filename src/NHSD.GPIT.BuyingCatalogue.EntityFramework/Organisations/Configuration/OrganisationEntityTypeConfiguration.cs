using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Configuration
{
    internal sealed class OrganisationEntityTypeConfiguration : IEntityTypeConfiguration<Organisation>
    {
        public void Configure(EntityTypeBuilder<Organisation> builder)
        {
            builder.ToTable("Organisations", Schemas.Organisations);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Address)
                .HasConversion(
                    a => JsonSerializer.Serialize(a, (JsonSerializerOptions)null),
                    a => JsonSerializer.Deserialize<Address>(a, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.LastUpdated).HasDefaultValue(DateTime.UtcNow);
            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(o => o.ExternalIdentifier).HasMaxLength(100);

            builder.Property(o => o.InternalIdentifier).HasMaxLength(103);

            builder.Property(o => o.OrganisationType)
                .HasConversion<int>()
                .HasColumnName("OrganisationTypeId");

            builder.Property(o => o.OrganisationRoleId).HasMaxLength(8);

            builder.HasIndex(o => o.Name, "AK_Organisations_Name")
                .IsUnique();

            builder.HasOne(o => o.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(o => o.LastUpdatedBy)
                .HasConstraintName("FK_Organisations_LastUpdatedBy");
        }
    }
}
