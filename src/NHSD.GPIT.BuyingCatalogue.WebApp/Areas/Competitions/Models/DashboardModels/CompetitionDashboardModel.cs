using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

public sealed class CompetitionDashboardModel
{
    public CompetitionDashboardModel(
        string organisationName,
        IEnumerable<Competition> competitions)
    {
        OrganisationName = organisationName;

        Competitions = competitions.Select(
                x => new CompetitionDashboardItem(x.Id, x.Name, x.Description, x.LastUpdated, x.Completed))
            .ToList();
    }

    public string OrganisationName { get; set; }

    public List<CompetitionDashboardItem> Competitions { get; set; }
}
