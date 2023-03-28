using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Configuration;

public class OrganisationRoleEntityTypeConfiguration : IEntityTypeConfiguration<OrganisationRole>
{
    public void Configure(EntityTypeBuilder<OrganisationRole> builder)
    {
        builder.ToTable("OrganisationRoles", schema: Schemas.OdsOrganisations);

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Organisation)
            .WithMany(x => x.Roles)
            .HasForeignKey(x => x.OrganisationId);

        builder.HasOne(x => x.RoleType)
            .WithMany()
            .HasForeignKey(x => x.RoleId);

        builder.Property(x => x.IsPrimaryRole)
            .IsRequired();

        builder.HasIndex(x => x.RoleId)
            .HasDatabaseName("IX_RoleId_OrganisationId")
            .IncludeProperties(x => x.OrganisationId)
            .IsClustered(false);

        builder.HasIndex(x => new { x.OrganisationId, x.IsPrimaryRole })
            .HasDatabaseName("IX_IsPrimaryRole_OrganisationId")
            .IsClustered(false);
    }
}
