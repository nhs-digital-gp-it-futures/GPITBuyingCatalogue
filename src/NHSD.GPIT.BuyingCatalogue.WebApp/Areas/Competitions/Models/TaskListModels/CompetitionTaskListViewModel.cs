using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

public class CompetitionTaskListViewModel : NavBaseModel
{
    public CompetitionTaskListViewModel(
        Organisation organisation,
        CompetitionTaskListModel model)
    {
        OrganisationName = organisation.Name;
        InternalOrgId = organisation.InternalIdentifier;

        TaskListModel = model;
    }

    public string OrganisationName { get; set; }

    public string InternalOrgId { get; set; }

    public CompetitionTaskListModel TaskListModel { get; set; }
}
