using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Configuration;

public class OrgImportJournalEntityTypeConfiguration : IEntityTypeConfiguration<OrgImportJournal>
{
    public void Configure(EntityTypeBuilder<OrgImportJournal> builder)
    {
        builder.ToTable("Journal", schema: Schemas.OdsOrganisations);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ImportDate).IsRequired().HasDefaultValue(DateTime.UtcNow).ValueGeneratedOnAdd();
    }
}
