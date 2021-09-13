using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels
{
    public sealed class EditSupplierDetailsModel : NavBaseModel
    {
        public EditSupplierDetailsModel()
        {
            Title = "Supplier details";
        }

        public EditSupplierDetailsModel(Supplier supplier)
        {
            Title = $"{supplier.Name} details";

            SupplierName = supplier.Name;
            SupplierLegalName = supplier.LegalName;
            AboutSupplier = supplier.Summary;
            SupplierWebsite = supplier.SupplierUrl;
        }

        [Required(ErrorMessage = "Enter a supplier name")]
        [StringLength(255)]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Enter a supplier legal name")]
        [StringLength(255)]
        public string SupplierLegalName { get; set; }

        [StringLength(1100)]
        public string AboutSupplier { get; set; }

        [Url]
        [StringLength(1000)]
        public string SupplierWebsite { get; set; }

        public string Title { get; init; }
    }
}
