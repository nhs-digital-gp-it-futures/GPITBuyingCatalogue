using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

public sealed class CompetitionDashboardModel
{
    public CompetitionDashboardModel(
        string internalOrgId,
        string organisationName,
        IEnumerable<Competition> competitions)
    {
        InternalOrgId = internalOrgId;

        OrganisationName = organisationName;

        Competitions = competitions.Select(
                x => new CompetitionDashboardItem(x))
            .ToList();
    }

    public string InternalOrgId { get; set; }

    public string OrganisationName { get; set; }

    public List<CompetitionDashboardItem> Competitions { get; set; }

    public PageOptions Options { get; set; }
}
