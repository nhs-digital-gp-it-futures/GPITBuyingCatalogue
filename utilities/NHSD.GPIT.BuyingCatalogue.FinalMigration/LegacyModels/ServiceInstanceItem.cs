using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class ServiceInstanceItem
    {
        public int OrderId { get; set; }
        public string CatalogueItemId { get; set; }
        public string OdsCode { get; set; }
        public string ServiceInstanceId { get; set; }
    }
}
