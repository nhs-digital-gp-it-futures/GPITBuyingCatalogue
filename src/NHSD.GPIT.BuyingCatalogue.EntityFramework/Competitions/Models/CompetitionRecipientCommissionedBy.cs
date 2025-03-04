using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public record CompetitionRecipientCommissionedBy
{
    public int CompetitionId { get; set; }

    [MaxLength(10)]
    public string RecipientOdsCode { get; set; }

    [MaxLength(10)]
    public string CommissionedByOdsCode { get; set; }

    public Competition Competition { get; set; }

    public CompetitionRecipient CompetitionRecipient { get; set; }

    public CompetitionSublocations CommissionedByCompetitionSublocations { get; set; }
}
