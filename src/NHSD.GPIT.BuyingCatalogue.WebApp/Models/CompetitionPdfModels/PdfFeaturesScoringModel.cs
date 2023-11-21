using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionPdfModels;

public class PdfFeaturesScoringModel : FeaturesScoringModel
{
    public PdfFeaturesScoringModel(Competition competition)
        : base(competition)
    {
        Competition = competition;
    }

    public Competition Competition { get; set; }
}
