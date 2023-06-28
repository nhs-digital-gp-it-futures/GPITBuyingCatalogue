namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

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
