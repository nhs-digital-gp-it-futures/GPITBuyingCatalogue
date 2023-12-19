using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class EditAssociatedServiceDetailsModel : NavBaseModel
    {
        public EditAssociatedServiceDetailsModel()
        {
        }

        public EditAssociatedServiceDetailsModel(int supplierId, string supplierName, CatalogueItem associatedServiceItem, List<SolutionMergerAndSplitTypesModel> list)
        {
            Id = associatedServiceItem.Id;
            ServiceName = Name = associatedServiceItem.Name;
            SupplierName = supplierName;
            SupplierId = supplierId;
            Description = associatedServiceItem.AssociatedService.Description;
            OrderGuidance = associatedServiceItem.AssociatedService.OrderGuidance;
            PracticeMerger = (associatedServiceItem.AssociatedService.PracticeReorganisationType & PracticeReorganisationTypeEnum.Merger) == PracticeReorganisationTypeEnum.Merger;
            PracticeSplit = (associatedServiceItem.AssociatedService.PracticeReorganisationType & PracticeReorganisationTypeEnum.Split) == PracticeReorganisationTypeEnum.Split;
            SolutionMergerAndSplits = list;

            HaveCorrectProvisioningAndCalculationTypes =
                associatedServiceItem.CataloguePrices.All(x =>
                    x.ProvisioningType == ProvisioningType.Declarative
                    && x.CataloguePriceQuantityCalculationType == CataloguePriceQuantityCalculationType.PerServiceRecipient
                    && x.CataloguePriceCalculationType == CataloguePriceCalculationType.Volume);

            NotHaveTieredPrices =
                associatedServiceItem.CataloguePrices.All(x =>
                    x.CataloguePriceType != CataloguePriceType.Tiered);
        }

        public CatalogueItemId? Id { get; init; }

        public string ServiceName { get; set; }

        public string SupplierName { get; set; }

        public int SupplierId { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(1000)]
        public string OrderGuidance { get; set; }

        public bool PracticeSplit { get; set; }

        public bool HaveCorrectProvisioningAndCalculationTypes { get; set; }

        public bool NotHaveTieredPrices { get; set; }

        public bool PracticeMerger { get; set; }

        public List<SolutionMergerAndSplitTypesModel> SolutionMergerAndSplits { get; set; } = new();

        public PracticeReorganisationTypeEnum PracticeReorganisation => (PracticeMerger ? PracticeReorganisationTypeEnum.Merger : PracticeReorganisationTypeEnum.None)
            | (PracticeSplit ? PracticeReorganisationTypeEnum.Split : PracticeReorganisationTypeEnum.None);
    }
}
