using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

public class ConfirmResultsModel : NavBaseModel
{
    public ConfirmResultsModel()
    {
    }

    public ConfirmResultsModel(
        Competition competition)
    {
        CompetitionName = competition.Name;
        CompetitionSolutions = competition.CompetitionSolutions;
        NonPriceElements = competition.NonPriceElements.GetNonPriceElements().ToList();

        IncludesNonPriceElements = competition.IncludesNonPrice.GetValueOrDefault();
    }

    public string InternalOrgId { get; set; }

    public string CompetitionName { get; set; }

    public bool IncludesNonPriceElements { get; set; }

    public ICollection<NonPriceElement> NonPriceElements { get; set; }

    public ICollection<CompetitionSolution> CompetitionSolutions { get; set; }

    public string PdfUrl { get; set; }
}
