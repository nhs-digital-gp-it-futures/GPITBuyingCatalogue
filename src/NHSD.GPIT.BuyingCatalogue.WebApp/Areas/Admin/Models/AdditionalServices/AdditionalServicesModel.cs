using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices
{
    public sealed class AdditionalServicesModel : NavBaseModel
    {
        public AdditionalServicesModel()
        {
        }

        public AdditionalServicesModel(CatalogueItem catalogueItem, IReadOnlyList<CatalogueItem> additionalServices)
            : this()
        {
            ItemId = catalogueItem.Id;
            ItemName = catalogueItem.Name;
            AdditionalServices = additionalServices;
        }

        public CatalogueItemId ItemId { get; init; }

        public string ItemName { get; init; }

        public IReadOnlyList<CatalogueItem> AdditionalServices { get; init; }

        public TaskProgress Status()
            => AdditionalServices.Any(i => i.PublishedStatus == PublicationStatus.Published)
               ? TaskProgress.Completed
               : TaskProgress.Optional;
    }
}
