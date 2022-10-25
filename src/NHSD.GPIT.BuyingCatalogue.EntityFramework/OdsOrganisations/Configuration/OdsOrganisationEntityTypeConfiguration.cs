﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Configuration;

public class OdsOrganisationEntityTypeConfiguration : IEntityTypeConfiguration<OdsOrganisation>
{
    public void Configure(EntityTypeBuilder<OdsOrganisation> builder)
    {
        builder.ToTable("OdsOrganisations", schema: "ods_organisations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("OrganisationId");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);

        builder.Property(x => x.AddressLine1).HasMaxLength(255);
        builder.Property(x => x.AddressLine2).HasMaxLength(255);
        builder.Property(x => x.AddressLine3).HasMaxLength(255);
        builder.Property(x => x.AddressLine4).HasMaxLength(255);
        builder.Property(x => x.AddressLine5).HasMaxLength(255);

        builder.Property(x => x.Town).HasMaxLength(100);
        builder.Property(x => x.County).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.Postcode).HasMaxLength(10);

        builder.Property(x => x.Status).IsRequired().HasMaxLength(100);
    }
}
