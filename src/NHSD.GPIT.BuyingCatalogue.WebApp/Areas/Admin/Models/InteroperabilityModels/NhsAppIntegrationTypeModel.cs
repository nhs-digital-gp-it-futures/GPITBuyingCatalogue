using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class NhsAppIntegrationTypeModel
    {
        public string IntegrationType { get; set; }

        public bool Checked { get; set; }
    }
}
