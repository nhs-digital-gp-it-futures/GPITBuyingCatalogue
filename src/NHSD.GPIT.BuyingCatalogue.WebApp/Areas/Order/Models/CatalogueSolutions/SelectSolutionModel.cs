using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public sealed class SelectSolutionModel : OrderingBaseModel
    {
        public SelectSolutionModel()
        {
        }

        public SelectSolutionModel(string odsCode, CallOffId callOffId, List<CatalogueItem> solutions, CatalogueItemId? selectedSolutionId)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions";
            Title = $"Add Catalogue Solution for {callOffId}";
            OdsCode = odsCode;
            Solutions = solutions;
            SelectedSolutionId = selectedSolutionId;
        }

        public List<CatalogueItem> Solutions { get; set; }

        [Required(ErrorMessage = "Select a Catalogue Solution")]
        public CatalogueItemId? SelectedSolutionId { get; set; }
    }
}
