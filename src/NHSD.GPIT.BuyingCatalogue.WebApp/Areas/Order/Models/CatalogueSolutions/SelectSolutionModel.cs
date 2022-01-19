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
            Title = $"Add a Catalogue Solution for {callOffId}";
            OdsCode = odsCode;
            Solutions = solutions;
            SelectedSolutionId = selectedSolutionId;
        }

        public List<CatalogueItem> Solutions { get; set; }

        public CatalogueItemId? SelectedSolutionId { get; set; }
    }
}
