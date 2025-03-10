using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public record CompetitionSublocationRecipient
{
    public int CompetitionId { get; set; }

    [MaxLength(10)]
    public string RecipientOdsCode { get; set; }

    [MaxLength(10)]
    public string ParentSublocationOdsCode { get; set; }

    public Competition Competition { get; set; }

    public CompetitionSublocation ParentSublocation { get; set; }

    public OdsOrganisation RecipientOrganisation { get; set; }
}
