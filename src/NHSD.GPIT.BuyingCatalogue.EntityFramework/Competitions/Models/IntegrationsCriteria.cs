using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

[ExcludeFromCodeCoverage]
public class IntegrationsCriteria
{
    public int NonPriceElementsId { get; set; }

    public int IntegrationTypeId { get; set; }

    public NonPriceElements NonPriceElements { get; set; }

    public IntegrationType IntegrationType { get; set; }
}
