using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public record CompetitionSublocation
{
    public int CompetitionId { get; set; }

    [MaxLength(10)]
    public string SublocationOdsCode { get; set; }

    [MaxLength(10)]
    public string OwnerOdsCode { get; set; }

    public bool Selected { get; set; } // could potentially remove and count presence as selected status

    public Competition Competition { get; set; }

    public ICollection<CompetitionSublocationRecipient> SublocationRecipients { get; set; }

    public OdsOrganisation SublocationOrganisation { get; set; }
}
