using System.Collections.Generic;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage
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

        public IList<SelectableOrderItemType> AvailableOrderItemTypes => new List<SelectableOrderItemType>
        {
            new SelectableOrderItemType(
                CatalogueItemType.Solution.AsString(EnumFormat.DisplayName),
                "These are clinical IT systems that have meet the necessary requirements to feature on the Buying Catalogue.",
                CatalogueItemType.Solution),
            new SelectableOrderItemType(
                CatalogueItemType.AssociatedService.AsString(EnumFormat.DisplayName),
                "These are products that support the implementation of a solution and can be bought independently. For example, training or data migration or arranging the merging or splitting of existing practices.",
                CatalogueItemType.AssociatedService),
        };

        public struct SelectableOrderItemType
        {
            public SelectableOrderItemType(
                string name,
                string advice,
                CatalogueItemType value)
            {
                Name = name;
                Advice = advice;
                Value = value;
            }

            public string Name { get; set; }

            public string Advice { get; set; }

            public CatalogueItemType Value { get; set; }
        }
    }
}
