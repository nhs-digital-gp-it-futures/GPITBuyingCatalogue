using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public record CompetitionSublocation : ISublocation
{
    public int CompetitionId { get; set; }

    public string SublocationOdsCode { get; set; }

    public string OwnerOdsCode { get; set; }

    public Competition Competition { get; set; }

    public ICollection<CompetitionSublocationRecipient> SublocationRecipients { get; set; }

    public OdsOrganisation SublocationOrganisation { get; set; }
}
