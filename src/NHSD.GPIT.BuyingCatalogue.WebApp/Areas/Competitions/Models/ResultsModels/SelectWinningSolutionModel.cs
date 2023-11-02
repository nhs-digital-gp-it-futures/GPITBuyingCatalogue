using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

public class SelectWinningSolutionModel : NavBaseModel
{
    public SelectWinningSolutionModel()
    {
    }

    public SelectWinningSolutionModel(
        string competitionName,
        IEnumerable<Solution> winningSolutions)
    {
        CompetitionName = competitionName;
        WinningSolutions = winningSolutions.OrderBy(x => x.CatalogueItem.Name)
            .Select(
                x => new SelectOption<CatalogueItemId>(
                    x.CatalogueItem.Name,
                    x.CatalogueItem.Supplier.LegalName,
                    x.CatalogueItemId))
            .ToList();
    }

    public string CompetitionName { get; set; }

    public CatalogueItemId? SolutionId { get; set; }

    public List<SelectOption<CatalogueItemId>> WinningSolutions { get; set; }
}
