using System.Diagnostics.CodeAnalysis;

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
}
