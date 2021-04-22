using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation
{
    public class AboutSupplierModel : NavBaseModel
    { 
        public AboutSupplierModel()
        {
        }

        public AboutSupplierModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}";
            BackLinkText = "Return to all sections";

            SolutionId = catalogueItem.CatalogueItemId;
            SupplierId = catalogueItem.Supplier.Id;
            Description = catalogueItem.Supplier.Summary;
            Link = catalogueItem.Supplier.SupplierUrl;
        }

        public string SolutionId { get; set; }

        public string SupplierId { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }               
    }
}
