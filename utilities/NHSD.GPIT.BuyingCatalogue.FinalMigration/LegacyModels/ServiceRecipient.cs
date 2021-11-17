using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class ServiceRecipient
    {
        public string OdsCode { get; set; }
        public string Name { get; set; }        
    }
}
