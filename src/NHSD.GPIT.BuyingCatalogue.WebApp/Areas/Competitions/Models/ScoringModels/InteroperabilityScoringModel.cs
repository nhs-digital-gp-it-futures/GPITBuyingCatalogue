using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
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
            .WithInteroperability(competition.NonPriceElements.Interoperability);
    }

    public string CompetitionName { get; set; }

    public List<InteroperabilitySolutionScoreModel> SolutionScores { get; set; }

    public List<InteroperabilityCriteria> InteroperabilityCriteria { get; set; }

    public string PdfUrl { get; set; }

    public List<string> GetIm1Integrations() =>
        InteroperabilityCriteria.Where(
                x => x.IntegrationType == InteropIntegrationType.Im1
                    && Interoperability.Im1Integrations.ContainsKey(x.Qualifier))
            .Select(x => Interoperability.Im1Integrations[x.Qualifier])
            .ToList();

    public List<string> GetGpConnectIntegrations() =>
        InteroperabilityCriteria.Where(
                x => x.IntegrationType == InteropIntegrationType.GpConnect
                    && Interoperability.GpConnectIntegrations.ContainsKey(x.Qualifier))
            .Select(x => Interoperability.GpConnectIntegrations[x.Qualifier])
            .ToList();

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

    public InteroperabilityScoringModel WithInteroperability(IEnumerable<InteroperabilityCriteria> interoperability)
    {
        InteroperabilityCriteria = interoperability.ToList();

        return this;
    }
}
