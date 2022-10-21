using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Configuration;

public class OrganisationRoleEntityTypeConfiguration : IEntityTypeConfiguration<OrganisationRole>
{
    public void Configure(EntityTypeBuilder<OrganisationRole> builder)
    {
        builder.ToTable("OrganisationRoles", schema: "ods_organisations");

        builder.HasKey(x => x.UniqueRoleId);

        builder.HasOne(x => x.Organisation)
            .WithMany()
            .HasForeignKey(x => x.OrganisationId);

        builder.HasOne(x => x.RoleType)
            .WithMany()
            .HasForeignKey(x => x.RoleId);

        builder.Property(x => x.IsPrimaryRole)
            .IsRequired();
    }
}
