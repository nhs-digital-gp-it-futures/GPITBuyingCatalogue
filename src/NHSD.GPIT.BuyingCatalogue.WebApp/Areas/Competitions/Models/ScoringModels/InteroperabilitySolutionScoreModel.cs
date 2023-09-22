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
        var integrations = Solution.GetIntegrations().ToList();

        Im1Integrations = GetIntegrationType(integrations, Framework.Constants.Interoperability.IM1IntegrationType);
        GpConnectIntegrations = GetIntegrationType(integrations, Framework.Constants.Interoperability.GpConnectIntegrationType);
    }

    public List<(string Qualifier, List<Integration> Integrations)> Im1Integrations { get; set; }

    public List<(string Qualifier, List<Integration> Integrations)> GpConnectIntegrations { get; set; }

    private static List<(string Qualifier, List<Integration> Integrations)>
        GetIntegrationType(IEnumerable<Integration> integrations, string type)
    {
        var integrationsDict = GetIntegrationDictionary(type);

        return integrations
            .Where(
                x => x.IntegrationType.EqualsIgnoreCase(
                    type))
            .GroupBy(x => x.Qualifier)
            .Where(x => integrationsDict.ContainsKey(x.Key))
            .Select(x => (integrationsDict[x.Key], x.ToList()))
            .OrderBy(x => x.Item1)
            .ToList();
    }

    private static Dictionary<string, string> GetIntegrationDictionary(string type) =>
        type.EqualsIgnoreCase(Framework.Constants.Interoperability.IM1IntegrationType)
            ? Framework.Constants.Interoperability.Im1Integrations
            : Framework.Constants.Interoperability.GpConnectIntegrations;
}
