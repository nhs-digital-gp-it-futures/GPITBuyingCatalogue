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

            builder.HasKey(o => o.OrganisationId)
                .IsClustered(false);

            builder.Property(o => o.Address)
                .HasConversion(
                    a => JsonSerializer.Serialize(a, null),
                    a => JsonSerializer.Deserialize<Address>(a, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

            builder.Property(o => o.OrganisationId).ValueGeneratedNever();
            builder.Property(o => o.LastUpdated).HasDefaultValueSql("(getutcdate())");
            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(o => o.OdsCode).HasMaxLength(8);
            builder.Property(o => o.PrimaryRoleId).HasMaxLength(8);

            builder.HasIndex(o => o.Name, "IX_OrganisationName")
                .IsClustered();
        }
    }
}
