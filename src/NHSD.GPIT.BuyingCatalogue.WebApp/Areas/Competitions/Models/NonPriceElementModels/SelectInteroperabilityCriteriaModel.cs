using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
        Competition competition,
        IEnumerable<Integration> integrations)
    {
        CompetitionName = competition.Name;

        CanDelete = competition.NonPriceElements?.IntegrationTypes?.Count == 0;

        Integrations = integrations.Where(x => x.Id is SupportedIntegrations.Im1 or SupportedIntegrations.GpConnect) // CM TODO: Make generic when introducing NHS App Integration Types to Competition journey
            .Select(
                x => (x.Name,
                    x.IntegrationTypes.Select(
                            y => new SelectOption<int>(
                                y.Name,
                                y.Id,
                                competition.NonPriceElements?.IntegrationTypes?.Any(z => z.Id == y.Id) ?? false))
                        .ToList()))
            .ToList();
    }

    public string CompetitionName { get; set; }

    public List<(string Name, List<SelectOption<int>> IntegrationTypes)> Integrations { get; set; }
}
