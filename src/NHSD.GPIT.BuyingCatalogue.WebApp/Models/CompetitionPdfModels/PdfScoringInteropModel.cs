using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionPdfModels;

public class PdfScoringInteropModel : InteroperabilityScoringModel
{
    public PdfScoringInteropModel(
        Competition competition,
        IEnumerable<Integration> availableIntegrations)
        : base(competition, availableIntegrations)
    {
        Competition = competition;
    }

    public Competition Competition { get; set; }
}
