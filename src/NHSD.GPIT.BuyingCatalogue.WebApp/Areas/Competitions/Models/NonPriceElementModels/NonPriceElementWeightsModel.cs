using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class NonPriceElementWeightsModel : NavBaseModel
{
    public NonPriceElementWeightsModel()
    {
    }

    public NonPriceElementWeightsModel(Competition competition)
    {
        CompetitionName = competition.Name;

        HasInteroperability = competition.NonPriceElements.HasNonPriceElement(NonPriceElement.Interoperability);
        HasImplementation = competition.NonPriceElements.HasNonPriceElement(NonPriceElement.Implementation);
        HasServiceLevel = competition.NonPriceElements.HasNonPriceElement(NonPriceElement.ServiceLevel);

        Implementation = competition.NonPriceElements.GetNonPriceWeight(NonPriceElement.Implementation);
        Interoperability = competition.NonPriceElements.GetNonPriceWeight(NonPriceElement.Interoperability);
        ServiceLevel = competition.NonPriceElements.GetNonPriceWeight(NonPriceElement.ServiceLevel);
    }

    public string CompetitionName { get; set; }

    [Description("Implementation weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? Implementation { get; set; }

    [Description("Interoperability weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? Interoperability { get; set; }

    [Description("Service level weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? ServiceLevel { get; set; }

    public bool HasImplementation { get; set; }

    public bool HasInteroperability { get; set; }

    public bool HasServiceLevel { get; set; }
}
