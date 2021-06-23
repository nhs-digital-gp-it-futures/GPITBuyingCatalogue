using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class SelectAssociatedServiceModel : OrderingBaseModel
    {
        public SelectAssociatedServiceModel()
        {
        }

        public SelectAssociatedServiceModel(string odsCode, string callOffId, List<CatalogueItem> solutions, string selectedSolutionId)
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
        public string SelectedSolutionId { get; set; }
    }
}
