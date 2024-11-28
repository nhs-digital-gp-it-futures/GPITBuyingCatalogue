using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared.Partials;

public class InteroperabilityPartialModel
{
    [ExcludeFromCodeCoverage]
    public InteroperabilityPartialModel()
    {
    }

    public InteroperabilityPartialModel(
        Dictionary<SupportedIntegrations, string> availableIntegrations,
        ICollection<IntegrationType> integrationTypes)
    {
        AvailableIntegrations = availableIntegrations;
        IntegrationTypes = integrationTypes.ToList();
    }

    public Dictionary<SupportedIntegrations, string> AvailableIntegrations { get; set; }

    public List<IntegrationType> IntegrationTypes { get; set; }
}
