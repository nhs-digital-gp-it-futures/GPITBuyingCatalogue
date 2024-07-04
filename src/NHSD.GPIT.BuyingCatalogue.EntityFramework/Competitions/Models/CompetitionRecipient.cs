using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

[ExcludeFromCodeCoverage(Justification = "Metadata class for EF Core")]
public class CompetitionRecipient
{
    public CompetitionRecipient()
    {
    }

    public CompetitionRecipient(
        int competitionId,
        string odsCode)
    {
        CompetitionId = competitionId;
        OdsCode = odsCode;
    }

    public int CompetitionId { get; set; }

    public string OdsCode { get; set; }

    public Competition Competition { get; set; }

    public OdsOrganisation OdsOrganisation { get; set; }
}
