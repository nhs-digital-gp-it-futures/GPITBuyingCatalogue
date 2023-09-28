using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;

public class NonPriceElementsPartialModel
{
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

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public NonPriceElements NonPriceElements { get; set; }

    public object RouteValues { get; set; }

    public List<string> GetIm1Integrations() =>
        NonPriceElements.Interoperability.Where(
                x => x.IntegrationType == InteropIntegrationType.Im1
                    && Interoperability.Im1Integrations.ContainsKey(x.Qualifier))
            .Select(x => Interoperability.Im1Integrations[x.Qualifier])
            .ToList();

    public List<string> GetGpConnectIntegrations() =>
        NonPriceElements.Interoperability.Where(
                x => x.IntegrationType == InteropIntegrationType.GpConnect
                    && Interoperability.GpConnectIntegrations.ContainsKey(x.Qualifier))
            .Select(x => Interoperability.GpConnectIntegrations[x.Qualifier])
            .ToList();
}
