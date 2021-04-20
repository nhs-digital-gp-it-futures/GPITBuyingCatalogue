using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using System;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class AboutSupplierModel
    { 
        public AboutSupplierModel()
        {
        }

        public AboutSupplierModel(CatalogueItem catalogueItem)
        {
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
