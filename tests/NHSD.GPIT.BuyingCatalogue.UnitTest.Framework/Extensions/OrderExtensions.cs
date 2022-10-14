using System.Linq;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Extensions
{
    public static class OrderExtensions
    {
        public static void SetupCatalogueSolution(this Order order)
        {
            if (order == null)
            {
                return;
            }

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.ElementAt(1).CatalogueItem.Name = "A";
            order.OrderItems.ElementAt(2).CatalogueItem.Name = "B";
        }
    }
}
