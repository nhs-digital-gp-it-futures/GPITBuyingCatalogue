using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Configuration;

public class OrganisationRelationshipEntityTypeConfiguration : IEntityTypeConfiguration<OrganisationRelationship>
{
    public void Configure(EntityTypeBuilder<OrganisationRelationship> builder)
    {
        builder.ToTable("OrganisationRelationships", schema: Schemas.OdsOrganisations);

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.RelationshipType)
            .WithMany()
            .HasForeignKey(x => x.RelationshipTypeId);

        builder.HasOne(x => x.OwnerOrganisation)
            .WithMany(x => x.Related)
            .HasForeignKey(x => x.OwnerOrganisationId);

        builder.HasOne(x => x.TargetOrganisation)
            .WithMany(x => x.Parents)
            .HasForeignKey(x => x.TargetOrganisationId);

        builder.HasIndex(x => new { x.RelationshipTypeId, x.OwnerOrganisationId, x.TargetOrganisationId })
            .HasDatabaseName("IX_RelationshipType_TargetOwnerOrganisationId")
            .IsClustered(false);
    }
}
