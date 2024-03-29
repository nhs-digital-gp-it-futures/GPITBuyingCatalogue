﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Configuration
{
    internal sealed class RelatedOrganisationEntityTypeConfiguration : IEntityTypeConfiguration<RelatedOrganisation>
    {
        public void Configure(EntityTypeBuilder<RelatedOrganisation> builder)
        {
            builder.ToTable("RelatedOrganisations", Schemas.Organisations);

            builder.HasKey(ro => new { ro.OrganisationId, ro.RelatedOrganisationId });

            builder.Property(ro => ro.LastUpdated).HasDefaultValue(DateTime.UtcNow);

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

            builder.HasOne(ro => ro.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(ro => ro.LastUpdatedBy)
                .HasConstraintName("FK_RelatedOrganisations_LastUpdatedBy");
        }
    }
}
