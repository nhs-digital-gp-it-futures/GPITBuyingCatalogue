#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class SupplierServiceAssociation
    {
        public string AssociatedServiceId { get; set; }

        public string CatalogueItemId { get; set; }

        public virtual AssociatedService AssociatedService { get; set; }

        public virtual CatalogueItem CatalogueItem { get; set; }
    }
}
