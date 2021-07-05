using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Configuration
{
    internal sealed class RelatedOrganisationEntityTypeConfiguration : IEntityTypeConfiguration<RelatedOrganisation>
    {
        public void Configure(EntityTypeBuilder<RelatedOrganisation> builder)
        {
            builder.HasKey(ro => new { ro.OrganisationId, ro.RelatedOrganisationId });

            builder.HasOne(ro => ro.Organisation)
                .WithMany(o => o.RelatedOrganisationOrganisations)
                .HasForeignKey(ro => ro.OrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RelatedOrganisations_OrganisationId");

            builder.HasOne(ro => ro.RelatedOrganisationNavigation)
                .WithMany(o => o.RelatedOrganisationRelatedOrganisationNavigations)
                .HasForeignKey(ro => ro.RelatedOrganisationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RelatedOrganisations_RelatedOrganisationId");
        }
    }
}
