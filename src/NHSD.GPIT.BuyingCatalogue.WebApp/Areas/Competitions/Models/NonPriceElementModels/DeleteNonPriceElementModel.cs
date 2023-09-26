using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class DeleteNonPriceElementModel : NavBaseModel
{
    public DeleteNonPriceElementModel()
    {
    }

    public DeleteNonPriceElementModel(NonPriceElement nonPriceElement)
    {
        NonPriceElement = nonPriceElement;
    }

    public NonPriceElement NonPriceElement { get; set; }
}
