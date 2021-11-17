using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class PricingUnit
    {       
        public string Name { get; set; }
        public string Description { get; set; }        
    }
}
