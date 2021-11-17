using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class Supplier
    {       
        public string Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
    }
}
