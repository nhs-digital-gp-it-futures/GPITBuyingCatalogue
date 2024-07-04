using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage
{
    public sealed class OrderItemTypeModel : NavBaseModel
    {
        public OrderItemTypeModel()
        {
        }

        public OrderItemTypeModel(string organisationName)
        {
            OrganisationName = organisationName;
        }

        public string OrganisationName { get; set; }

        public CatalogueItemType? SelectedOrderItemType { get; set; }

        public IList<SelectOption<CatalogueItemType>> AvailableOrderItemTypes => new List<SelectOption<CatalogueItemType>>
        {
            new(
                "Catalogue Solution and other services",
                "Order clinical IT systems and their related services.",
                CatalogueItemType.Solution),
            new(
                "Associated Service only",
                "Order products to support the implementation of a solution. For example, training or data migration or arranging the merging or splitting of existing practices.",
                CatalogueItemType.AssociatedService),
        };
    }
}
