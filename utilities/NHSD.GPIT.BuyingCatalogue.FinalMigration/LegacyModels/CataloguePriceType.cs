using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class CataloguePriceType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
