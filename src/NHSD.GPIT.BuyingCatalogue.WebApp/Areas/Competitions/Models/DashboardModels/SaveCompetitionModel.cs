using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

public class SaveCompetitionModel : NavBaseModel
{
    public SaveCompetitionModel()
    {
    }

    public SaveCompetitionModel(int organisationId, string organisationName)
    {
        OrganisationId = organisationId;
        OrganisationName = organisationName;
    }

    public int OrganisationId { get; set; }

    public string OrganisationName { get; set; }

    [StringLength(255)]
    public string Name { get; set; }

    [StringLength(250)]
    public string Description { get; set; }
}
