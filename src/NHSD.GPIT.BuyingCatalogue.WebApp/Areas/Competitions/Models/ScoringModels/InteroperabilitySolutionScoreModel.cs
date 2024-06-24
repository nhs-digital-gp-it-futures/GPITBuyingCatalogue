using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

public class InteroperabilitySolutionScoreModel : SolutionScoreModel
{
    public InteroperabilitySolutionScoreModel()
    {
    }

    public InteroperabilitySolutionScoreModel(
        Solution solution,
        int? score,
        string justification)
    : base(solution, score, justification)
    {
        var integrations = Solution.Integrations.ToList();

        Im1Integrations = GetIntegrationType(integrations, SupportedIntegrations.Im1);
        GpConnectIntegrations = GetIntegrationType(integrations, SupportedIntegrations.GpConnect);
    }

    public List<(string Qualifier, List<SolutionIntegration> Integrations)> Im1Integrations { get; set; }

    public List<(string Qualifier, List<SolutionIntegration> Integrations)> GpConnectIntegrations { get; set; }

    private static List<(string Qualifier, List<SolutionIntegration> Integrations)>
        GetIntegrationType(IEnumerable<SolutionIntegration> integrations, SupportedIntegrations integration)
    {
        return integrations
            .Where(
                x => x.IntegrationType.IntegrationId == integration)
            .GroupBy(x => x.IntegrationType.Name)
            .Select(x => (x.Key, x.ToList()))
            .OrderBy(x => x.Key)
            .ToList();
    }
}
