using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class MatchDatesModel : NavBaseModel
    {
        public const string YesOption = "Yes";
        public const string NoOption = "No";

        public MatchDatesModel()
        {
        }

        public MatchDatesModel(string internalOrgId, CallOffId callOffId, CatalogueItem catalogueItem)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            ItemType = catalogueItem.CatalogueItemType.Name();
            ItemName = catalogueItem.Name;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string ItemType { get; set; }

        public string ItemName { get; set; }

        public bool? MatchDates { get; set; }

        public IEnumerable<SelectOption<bool>> Options => new List<SelectOption<bool>>
        {
            new(YesOption, true),
            new(NoOption, false),
        };
    }
}
