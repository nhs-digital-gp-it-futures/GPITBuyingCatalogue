using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage
{
    public class DetermineAssociatedServiceTypeModel : NavBaseModel
    {
        public DetermineAssociatedServiceTypeModel()
        {
        }

        public DetermineAssociatedServiceTypeModel(string organisationName)
        {
            OrganisationName = organisationName;
        }

        public string OrganisationName { get; set; }

        public OrderTypeEnum? OrderType { get; set; }

        public IList<SelectOption<OrderTypeEnum>> AvailableOrderTypes => new List<SelectOption<OrderTypeEnum>>
        {
            new(
                "Merger",
                "This is where 2 or more practices join to become a single practice with a combined practice li​st size.",
                OrderTypeEnum.AssociatedServiceMerger),
            new(
                "Split",
                "This is where 1 practice becomes 2 or more practices and the original practice list is shared between each party.",
                OrderTypeEnum.AssociatedServiceSplit),
            new(
                "Something else",
                "This is for ordering any other type of Associated Service.",
                OrderTypeEnum.AssociatedServiceOther),
        };
    }
}
