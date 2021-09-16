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
        public string SupplierName { get; init; }

        [Required(ErrorMessage = "Enter a supplier legal name")]
        [StringLength(255)]
        public string SupplierLegalName { get; init; }

        [StringLength(1100)]
        public string AboutSupplier { get; init; }

        [Url]
        [StringLength(1000)]
        public string SupplierWebsite { get; init; }

        public string Title { get; }
    }
}
