using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionResultsPdf;

public class PdfViewResultsModel : ViewResultsModel
{
    public PdfViewResultsModel(Competition competition, FilterDetailsModel filterDetails, ICollection<CompetitionSolution> nonShortlistedSolutions)
        : base(competition, filterDetails, nonShortlistedSolutions)
    {
        Competition = competition;

        FilterDetailsModel = new ReviewFilterModel(filterDetails);
    }

    public Competition Competition { get; set; }

    public new ReviewFilterModel FilterDetailsModel { get; set; }

    public bool IsDirectAward() => Competition.CompetitionSolutions.Count + NonShortlistedSolutions.Count == 1;
}
