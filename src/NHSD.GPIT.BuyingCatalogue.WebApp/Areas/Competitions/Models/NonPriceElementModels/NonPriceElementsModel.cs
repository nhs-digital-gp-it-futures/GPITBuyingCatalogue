using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class NonPriceElementsModel : NavBaseModel
{
    public NonPriceElementsModel(
        Competition competition,
        IEnumerable<Integration> availableIntegrations)
    {
        InternalOrgId = competition.Organisation.InternalIdentifier;
        CompetitionId = competition.Id;

        CompetitionName = competition.Name;
        HasReviewedCriteria = competition.HasReviewedCriteria;
        NonPriceElements = competition.NonPriceElements;
        AvailableIntegrations = availableIntegrations.ToDictionary(x => x.Id, x => x.Name);
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string CompetitionName { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public NonPriceElements NonPriceElements { get; set; }

    public Dictionary<SupportedIntegrations, string> AvailableIntegrations { get; set; }

    public override string Advice => HasReviewedCriteria
        ? "These are the non-price elements you added to help you score your shortlisted solutions."
        : HasAllNonPriceElements()
            ? "All available non-price elements have been added for this competition."
            : "Add at least 1 optional non-price element to help you score your shortlisted solutions, for example features, implementation, interoperability or service levels.";

    public bool HasAllNonPriceElements() =>
        NonPriceElements.HasAllNonPriceElements();

    public bool HasAnyNonPriceElements() =>
        NonPriceElements.HasAnyNonPriceElements();
}
