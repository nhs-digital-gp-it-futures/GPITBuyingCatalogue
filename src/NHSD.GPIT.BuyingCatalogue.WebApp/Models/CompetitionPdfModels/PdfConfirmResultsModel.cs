using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionPdfModels;

public class PdfConfirmResultsModel : ConfirmResultsModel
{
    public PdfConfirmResultsModel(Competition competition)
        : base(competition)
    {
        Competition = competition;
    }

    public Competition Competition { get; set; }
}
