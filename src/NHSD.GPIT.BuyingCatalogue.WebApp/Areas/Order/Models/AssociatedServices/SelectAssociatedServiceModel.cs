using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class SelectAssociatedServiceModel : OrderingBaseModel
    {
        public SelectAssociatedServiceModel()
        {
        }

        public SelectAssociatedServiceModel(string odsCode, CallOffId callOffId, List<CatalogueItem> solutions, CatalogueItemId? selectedSolutionId)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/associated-services";
            BackLinkText = "Go back";
            Title = $"Add Associated Service for {callOffId}";
            OdsCode = odsCode;
            Solutions = solutions;
            SelectedSolutionId = selectedSolutionId;
        }

        public List<CatalogueItem> Solutions { get; set; }

        [Required(ErrorMessage = "Select an Associated Service")]
        public CatalogueItemId? SelectedSolutionId { get; set; }
    }
}
