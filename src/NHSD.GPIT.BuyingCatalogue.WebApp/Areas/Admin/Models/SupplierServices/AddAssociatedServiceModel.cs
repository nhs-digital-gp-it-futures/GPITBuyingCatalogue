using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierServices
{
    public sealed class AddAssociatedServiceModel : NavBaseModel
    {
        public AddAssociatedServiceModel()
        {
        }

        public AddAssociatedServiceModel(Supplier supplier)
        {
            SupplierId = supplier.Id;
            SupplierName = supplier.Name;
        }

        public int SupplierId { get; set; }

        public string SupplierName { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(1000)]
        public string OrderGuidance { get; set; }

        public bool PracticeSplit { get; set; }

        public bool PracticeMerger { get; set; }

        public PracticeReorganisationTypeEnum PracticeReorganisation => (PracticeMerger ? PracticeReorganisationTypeEnum.Merger : PracticeReorganisationTypeEnum.None)
            | (PracticeSplit ? PracticeReorganisationTypeEnum.Split : PracticeReorganisationTypeEnum.None);
    }
}
