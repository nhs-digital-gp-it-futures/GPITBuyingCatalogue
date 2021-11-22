using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class CatalogueItem
    {
        public string Id { get; set; }
        public int CatalogueItemTypeId { get; set; }
        public string Name { get; set; }
        public string ParentCatalogueItemId { get; set; }
    }
}
