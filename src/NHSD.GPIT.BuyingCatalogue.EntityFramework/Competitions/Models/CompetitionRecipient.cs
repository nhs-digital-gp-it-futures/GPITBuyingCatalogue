using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

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

    public Competition Competition { get; set; }

    public OdsOrganisation OdsOrganisation { get; set; }
}
