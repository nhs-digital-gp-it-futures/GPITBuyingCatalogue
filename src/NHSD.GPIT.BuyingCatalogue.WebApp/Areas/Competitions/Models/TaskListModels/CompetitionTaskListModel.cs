using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

public class CompetitionTaskListModel : NavBaseModel
{
    public CompetitionTaskListModel(
        Organisation organisation,
        Competition competition)
    {
        OrganisationName = organisation.Name;
        InternalOrgId = organisation.InternalIdentifier;

        CompetitionId = competition.Id;
        Name = competition.Name;
        Description = competition.Description;
    }

    public string OrganisationName { get; set; }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}
