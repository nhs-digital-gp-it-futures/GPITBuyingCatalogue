using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class SelectAdditionalServiceModel : OrderingBaseModel
    {
        public SelectAdditionalServiceModel()
        {
        }

        public SelectAdditionalServiceModel(string odsCode, string callOffId, List<CatalogueItem> solutions, string selectedSolutionId)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services";
            BackLinkText = "Go back";
            Title = $"Add Additional Service for {callOffId}";
            OdsCode = odsCode;
            Solutions = solutions;
            SelectedSolutionId = selectedSolutionId;
        }

        public List<CatalogueItem> Solutions { get; set; }

        [Required(ErrorMessage = "Select an Additional Service")]
        public string SelectedSolutionId { get; set; }
    }
}
