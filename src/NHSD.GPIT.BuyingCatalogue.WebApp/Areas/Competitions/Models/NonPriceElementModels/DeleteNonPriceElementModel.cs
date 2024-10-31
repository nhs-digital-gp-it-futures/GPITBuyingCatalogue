using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class DeleteNonPriceElementModel : NavBaseModel
{
    public DeleteNonPriceElementModel()
    {
    }

    public DeleteNonPriceElementModel(NonPriceElement nonPriceElement, Competition competition, IEnumerable<Integration> availableIntegrations = null, int featureId = 0)
    {
        InternalOrgId = competition.Organisation.InternalIdentifier;
        CompetitionId = competition.Id;

        CompetitionName = competition.Name;
        HasReviewedCriteria = competition.HasReviewedCriteria;
        NonPriceElement = nonPriceElement;
        NonPriceElements = featureId == 0 ? competition.NonPriceElements : new NonPriceElements() { Features = competition.NonPriceElements.Features.Where(x => x.Id == featureId).ToList() };
        AvailableIntegrations = availableIntegrations?.ToDictionary(x => x.Id, x => x.Name);
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string CompetitionName { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public NonPriceElement NonPriceElement { get; set; }

    public NonPriceElements NonPriceElements { get; set; }

    public Dictionary<SupportedIntegrations, string> AvailableIntegrations { get; set; }

    public string SingleElementDisplay => NonPriceElement == NonPriceElement.Features ? "Feature" : NonPriceElement.EnumMemberName();

    public override string Advice => NonPriceElement == NonPriceElement.Features ? "If you delete all your features requirements, features will be removed as a non-price element." :
        "Deleting this requirement will remove " + NonPriceElement.EnumMemberName().ToLower() + " as a non-price element.";
}
