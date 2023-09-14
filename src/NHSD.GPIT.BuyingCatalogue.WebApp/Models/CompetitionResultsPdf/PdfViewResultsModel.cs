using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionResultsPdf;

public class PdfViewResultsModel : ViewResultsModel
{
    public PdfViewResultsModel(Competition competition)
        : base(competition)
    {
        Competition = competition;
    }

    public Competition Competition { get; set; }
}
