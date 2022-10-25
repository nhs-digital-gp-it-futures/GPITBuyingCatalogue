using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Configuration;

public class OrganisationRelationshipEntityTypeConfiguration : IEntityTypeConfiguration<OrganisationRelationship>
{
    public void Configure(EntityTypeBuilder<OrganisationRelationship> builder)
    {
        builder.ToTable("OrganisationRelationships", schema: "ods_organisations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.RelationshipTypeId).HasColumnName("RelTypeId");

        builder.HasOne(x => x.RelationshipType)
            .WithMany()
            .HasForeignKey(x => x.RelationshipTypeId);

        builder.HasOne(x => x.TargetOrganisation)
            .WithOne(x => x.Parent)
            .HasForeignKey<OrganisationRelationship>(x => x.TargetOrganisationId);

        builder.HasOne(x => x.OwnerOrganisation)
            .WithMany(x => x.Related)
            .HasForeignKey(x => x.OwnerOrganisationId);
    }
}
