using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        HasReviewedCriteria = competition.HasReviewedCriteria;

        HasFeatures = competition.NonPriceElements.HasNonPriceElement(NonPriceElement.Features);
        HasInteroperability = competition.NonPriceElements.HasNonPriceElement(NonPriceElement.Interoperability);
        HasImplementation = competition.NonPriceElements.HasNonPriceElement(NonPriceElement.Implementation);
        HasServiceLevel = competition.NonPriceElements.HasNonPriceElement(NonPriceElement.ServiceLevel);

        Features = competition.NonPriceElements.GetNonPriceWeight(NonPriceElement.Features);
        Implementation = competition.NonPriceElements.GetNonPriceWeight(NonPriceElement.Implementation);
        Interoperability = competition.NonPriceElements.GetNonPriceWeight(NonPriceElement.Interoperability);
        ServiceLevel = competition.NonPriceElements.GetNonPriceWeight(NonPriceElement.ServiceLevel);

        NonPriceWeights = competition.NonPriceElements.GetNonPriceElements()
            .OrderBy(x => x.ToString())
            .ToDictionary(x => x, x => competition.NonPriceElements.GetNonPriceWeight(x));
    }

    public string CompetitionName { get; set; }

    [Description("Features weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? Features { get; set; }

    [Description("Implementation weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? Implementation { get; set; }

    [Description("Interoperability weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? Interoperability { get; set; }

    [Description("Service level weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? ServiceLevel { get; set; }

    public bool HasFeatures { get; set; }

    public bool HasImplementation { get; set; }

    public bool HasInteroperability { get; set; }

    public bool HasServiceLevel { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public Dictionary<NonPriceElement, int?> NonPriceWeights { get; set; }

    public string ContinueButton => HasReviewedCriteria
        ? "Continue"
        : "Save and continue";

    public override string Title => HasReviewedCriteria
        ? "Non-price weightings"
        : "How would you like to weight your non-price elements for this competition?";

    public override string Advice => HasReviewedCriteria
        ? "These are the non-price elements weightings you applied for this competition."
        : "Give your chosen non-price elements weightings based on how important they are to you.";
}
