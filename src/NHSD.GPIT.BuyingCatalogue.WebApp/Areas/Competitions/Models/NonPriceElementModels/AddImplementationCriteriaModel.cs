using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class AddImplementationCriteriaModel : NavBaseModel
{
    public AddImplementationCriteriaModel()
    {
    }

    public AddImplementationCriteriaModel(Competition competition)
    {
        CompetitionName = competition.Name;
        Requirements = competition.NonPriceElements?.Implementation?.Requirements;
    }

    public string CompetitionName { get; set; }

    [StringLength(1100)]
    public string Requirements { get; set; }
}
