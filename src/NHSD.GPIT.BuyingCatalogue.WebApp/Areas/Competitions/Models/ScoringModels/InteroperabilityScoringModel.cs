using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

public class InteroperabilityScoringModel : NavBaseModel
{
    public InteroperabilityScoringModel()
    {
    }

    public InteroperabilityScoringModel(
        Competition competition)
    {
        CompetitionName = competition.Name;

        WithSolutions(competition.CompetitionSolutions)
            .WithIntegrationTypes(competition.NonPriceElements.IntegrationTypes);
    }

    public string CompetitionName { get; set; }

    public List<InteroperabilitySolutionScoreModel> SolutionScores { get; set; }

    public List<IntegrationType> IntegrationTypes { get; set; }

    public string PdfUrl { get; set; }

    public InteroperabilityScoringModel WithSolutions(IEnumerable<CompetitionSolution> solutions, bool setScores = true)
    {
        SolutionScores = solutions.OrderBy(x => x.Solution.CatalogueItem.Name)
            .Select(
                x =>
                {
                    var score = x.GetScoreByType(ScoreType.Interoperability);

                    return new InteroperabilitySolutionScoreModel(
                        x.Solution,
                        setScores ? score?.Score : null,
                        score?.Justification);
                })
            .ToList();

        return this;
    }

    public InteroperabilityScoringModel WithIntegrationTypes(IEnumerable<IntegrationType> integrationTypes)
    {
        IntegrationTypes = integrationTypes.ToList();

        return this;
    }

    public ICollection<IntegrationType> GetIm1Integrations() =>
        IntegrationTypes.Where(x => x.IntegrationId == SupportedIntegrations.Im1).ToList();

    public ICollection<IntegrationType> GetGpConnectIntegrations() =>
        IntegrationTypes.Where(x => x.IntegrationId == SupportedIntegrations.GpConnect).ToList();
}
