using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class NonPriceElementBase : NavBaseModel
{
    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public bool CanDelete { get; set; }
}
