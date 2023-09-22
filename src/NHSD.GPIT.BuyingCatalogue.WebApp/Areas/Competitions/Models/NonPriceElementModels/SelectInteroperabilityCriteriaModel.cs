using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

public class SelectInteroperabilityCriteriaModel : NonPriceElementBase
{
    public SelectInteroperabilityCriteriaModel()
    {
    }

    public SelectInteroperabilityCriteriaModel(
        Competition competition)
    {
        CompetitionName = competition.Name;

        CanDelete = competition.NonPriceElements?.Interoperability?.Any() ?? false;

        Im1Integrations = GetIntegrationsSelectOptions(Interoperability.Im1Integrations, competition).ToList();

        GpConnectIntegrations =
            GetIntegrationsSelectOptions(Interoperability.GpConnectIntegrations, competition).ToList();
    }

    public string CompetitionName { get; set; }

    public List<SelectOption<string>> Im1Integrations { get; set; }

    public List<SelectOption<string>> GpConnectIntegrations { get; set; }

    private static IEnumerable<SelectOption<string>> GetIntegrationsSelectOptions(
        Dictionary<string, string> integrationsSet,
        Competition competition) => integrationsSet.Select(
            x => new SelectOption<string>(
                x.Value,
                x.Key,
                competition.NonPriceElements?.Interoperability?.Any(y => string.Equals(x.Key, y.Qualifier)) ?? false));
}
