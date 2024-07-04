using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

public class InteroperabilityScoringModel : NavBaseModel
{
    public InteroperabilityScoringModel()
    {
    }

    public InteroperabilityScoringModel(
        Competition competition,
        IEnumerable<Integration> integrations)
    {
        CompetitionName = competition.Name;

        WithSolutions(competition.CompetitionSolutions)
            .WithIntegrationTypes(competition.NonPriceElements.IntegrationTypes)
            .WithIntegrations(integrations);
    }

    public string CompetitionName { get; set; }

    public List<SolutionScoreModel> SolutionScores { get; set; }

    public List<IntegrationType> IntegrationTypes { get; set; }

    public Dictionary<SupportedIntegrations, string> AvailableIntegrations { get; set; }

    public string PdfUrl { get; set; }

    public InteroperabilityScoringModel WithSolutions(
        IEnumerable<CompetitionSolution> solutions,
        bool setScores = true)
    {
        SolutionScores = solutions.OrderBy(x => x.Solution.CatalogueItem.Name)
            .Select(
                x =>
                {
                    var score = x.GetScoreByType(ScoreType.Interoperability);

                    return new SolutionScoreModel(
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

    public InteroperabilityScoringModel WithIntegrations(IEnumerable<Integration> integrations)
    {
        AvailableIntegrations = integrations.ToDictionary(x => x.Id, x => x.Name);

        return this;
    }
}
