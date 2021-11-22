using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    [ExcludeFromCodeCoverage]
    public partial class OrderProgress
    {
        public int OrderId { get; set; }
        public bool CatalogueSolutionsViewed { get; set; }
        public bool AdditionalServicesViewed { get; set; }
        public bool AssociatedServicesViewed { get; set; }        
    }
}
