using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class SelectSolutionModel : OrderingBaseModel
    {
        public SelectSolutionModel()
        {
        }

        public SelectSolutionModel(string odsCode, string callOffId, List<CatalogueItem> solutions)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions";
            BackLinkText = "Go back";
            Title = $"Add Catalogue Solution for {callOffId}";
            OdsCode = odsCode;
            Solutions = solutions;
        }

        public List<CatalogueItem> Solutions { get; init; }

        [Required]
        public string SelectedSolutionId { get; set; }
    }
}
