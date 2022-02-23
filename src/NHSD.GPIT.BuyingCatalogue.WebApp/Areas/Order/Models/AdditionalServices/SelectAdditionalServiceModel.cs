using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class SelectAdditionalServiceModel : OrderingBaseModel
    {
        public SelectAdditionalServiceModel()
        {
        }

        public SelectAdditionalServiceModel(
            string odsCode,
            CallOffId callOffId,
            List<CatalogueItem> solutions,
            CatalogueItemId? selectedAdditionalServiceId)
        {
            Title = $"Add an Additional Service for {callOffId}";
            InternalOrgId = odsCode;
            Solutions = solutions;
            SelectedAdditionalServiceId = selectedAdditionalServiceId;
        }

        public List<CatalogueItem> Solutions { get; set; }

        public CatalogueItemId? SelectedAdditionalServiceId { get; set; }
    }
}
