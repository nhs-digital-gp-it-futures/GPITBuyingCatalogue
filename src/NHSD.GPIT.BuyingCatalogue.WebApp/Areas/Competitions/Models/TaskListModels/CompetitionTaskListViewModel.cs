using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
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

    public string AwardCriteriaText => TaskListModel.ReviewCompetitionCriteria is TaskProgress.Completed
        ? "View the award criteria you selected to help you compare your shortlisted solutions."
        : "Select the award criteria you want to use to compare your shortlisted solutions.";

    public string AwardCriteriaWeightingsText => TaskListModel.ReviewCompetitionCriteria is TaskProgress.Completed
        ? "View the award criteria weightings you gave for price and non-price elements."
        : "Give your chosen award criteria weightings based on how important they are to you.";

    public string NonPriceElementsText => TaskListModel.ReviewCompetitionCriteria is TaskProgress.Completed
        ? "View any non-price elements you added to help you score your shortlisted solutions."
        : "Select any non-price elements you want to add to help you score your shortlisted solutions.";

    public string NonPriceWeightingsText => TaskListModel.ReviewCompetitionCriteria is TaskProgress.Completed
        ? "View the non-price elements weightings you applied for this competition."
        : "Give your chosen non-price elements weightings based on how important they are to you.";
}
