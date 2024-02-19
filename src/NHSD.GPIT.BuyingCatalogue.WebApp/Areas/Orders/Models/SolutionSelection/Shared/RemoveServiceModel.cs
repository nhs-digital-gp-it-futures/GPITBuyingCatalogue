using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeStyle;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared
{
    public class RemoveServiceModel : NavBaseModel
    {
        public RemoveServiceModel()
        {
        }

        public RemoveServiceModel(
            CatalogueItem service)
        {
            ServiceName = service.Name;
            ServiceType = service.CatalogueItemType.DisplayName();
        }

        public IList<SelectOption<bool>> RemoveServiceOptions => new List<SelectOption<bool>>
        {
            new($"Yes, I confirm I want to remove {ServiceName}", true),
            new($"No, I want to keep my current {ServiceType}s", false),
        };

        public string ServiceType { get; set; }

        public string ServiceName { get; set; }

        public bool? ConfirmRemoveService { get; set; }
    }
}
