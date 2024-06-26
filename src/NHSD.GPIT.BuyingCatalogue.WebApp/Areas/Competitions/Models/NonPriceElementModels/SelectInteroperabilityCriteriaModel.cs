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

        Integrations = integrations
            .Select(
                x => new KeyValuePair<string, List<SelectOption<int>>>(
                    x.Name,
                    x.IntegrationTypes.Select(
                            y => new SelectOption<int>(
                                y.Name,
                                y.Id,
                                competition.NonPriceElements?.IntegrationTypes?.Any(z => z.Id == y.Id && z.IntegrationId == y.IntegrationId) ?? false))
                        .ToList()))
            .ToList();
    }

    public string CompetitionName { get; set; }

    public List<KeyValuePair<string, List<SelectOption<int>>>> Integrations { get; set; }
}
