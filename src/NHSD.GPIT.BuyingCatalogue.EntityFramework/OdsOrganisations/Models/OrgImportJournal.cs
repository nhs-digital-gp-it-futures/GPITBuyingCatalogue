using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

public class OrgImportJournal
{
    [ExcludeFromCodeCoverage]
    public OrgImportJournal()
    {
    }

    public OrgImportJournal(
        DateTime releaseDate)
    {
        ImportDate = releaseDate;
    }

    public int Id { get; set; }

    public DateTime ImportDate { get; set; }
}
