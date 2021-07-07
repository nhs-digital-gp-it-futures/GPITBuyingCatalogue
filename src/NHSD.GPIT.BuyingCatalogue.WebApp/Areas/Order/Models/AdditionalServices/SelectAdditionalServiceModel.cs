using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services";
            BackLinkText = "Go back";
            Title = $"Add Additional Service for {callOffId}";
            OdsCode = odsCode;
            Solutions = solutions;
            SelectedAdditionalServiceId = selectedAdditionalServiceId;
        }

        public List<CatalogueItem> Solutions { get; set; }

        [Required(ErrorMessage = "Select an Additional Service")]
        public CatalogueItemId? SelectedAdditionalServiceId { get; set; }
    }
}
