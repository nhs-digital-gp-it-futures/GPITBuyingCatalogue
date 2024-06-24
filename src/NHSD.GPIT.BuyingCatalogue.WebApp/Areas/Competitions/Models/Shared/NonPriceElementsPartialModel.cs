using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;

public class NonPriceElementsPartialModel
{
    [ExcludeFromCodeCoverage]
    public NonPriceElementsPartialModel()
    {
    }

    public NonPriceElementsPartialModel(
        string internalOrgId,
        int competitionId,
        NonPriceElements nonPriceElements,
        object routeValues)
    {
        InternalOrgId = internalOrgId;
        CompetitionId = competitionId;

        NonPriceElements = nonPriceElements;
        RouteValues = routeValues;
    }

    public NonPriceElementsPartialModel(
        string internalOrgId,
        int competitionId,
        NonPriceElements nonPriceElements,
        object routeValues,
        bool hasReviewedCriteria)
        : this(internalOrgId, competitionId, nonPriceElements, routeValues)
    {
        HasReviewedCriteria = hasReviewedCriteria;
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public NonPriceElements NonPriceElements { get; set; }

    public object RouteValues { get; set; }

    public bool IsReviewScreen { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public List<string> GetIm1Integrations() =>
        NonPriceElements.IntegrationTypes.Where(
                x => x.IntegrationId == SupportedIntegrations.Im1)
            .Select(x => x.Name)
            .ToList();

    public List<string> GetGpConnectIntegrations() =>
        NonPriceElements.IntegrationTypes.Where(
                x => x.IntegrationId == SupportedIntegrations.GpConnect)
            .Select(x => x.Name)
            .ToList();
}
