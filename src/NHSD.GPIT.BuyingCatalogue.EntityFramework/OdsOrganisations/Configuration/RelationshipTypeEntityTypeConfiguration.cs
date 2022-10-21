using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Configuration;

public class RelationshipTypeEntityTypeConfiguration : IEntityTypeConfiguration<RelationshipType>
{
    public void Configure(EntityTypeBuilder<RelationshipType> builder)
    {
        builder.ToTable("RelationshipTypes", schema: "ods_organisations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("RelTypeId");

        builder.Property(x => x.Description).IsRequired().HasMaxLength(100);
    }
}
