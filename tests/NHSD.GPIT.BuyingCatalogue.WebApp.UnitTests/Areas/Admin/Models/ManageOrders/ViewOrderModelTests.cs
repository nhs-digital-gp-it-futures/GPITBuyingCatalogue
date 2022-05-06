using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageOrders
{
    public static class ViewOrderModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Construct_WithOrderItems_DerivesFrameworkFromSolution(
            AspNetUser user,
            Organisation orderingParty,
            Supplier supplier,
            Solution solution,
            AssociatedService associatedService,
            AdditionalService additionalService,
            EntityFramework.Ordering.Models.Order order)
        {
            var framework = solution.FrameworkSolutions.First().Framework.ShortName;
            var orderItems = new List<OrderItem>
            {
                new() { CatalogueItem = solution.CatalogueItem },
                new() { CatalogueItem = associatedService.CatalogueItem },
                new() { CatalogueItem = additionalService.CatalogueItem },
            };

            orderItems.ForEach(oi => order.OrderItems.Add(oi));

            order.LastUpdatedByUser = user;
            order.OrderingParty = orderingParty;
            order.Supplier = supplier;

            var model = new ViewOrderModel(order);

            model.OrderItems.Should().AllSatisfy(oi => oi.Framework.Should().Be(framework));
        }

        [Theory]
        [CommonAutoData]
        public static void Construct_WithNoSolution_EmptyFrameworkName(
            AspNetUser user,
            Organisation orderingParty,
            Supplier supplier,
            AssociatedService associatedService,
            AdditionalService additionalService,
            EntityFramework.Ordering.Models.Order order)
        {
            var framework = string.Empty;
            var orderItems = new List<OrderItem>
            {
                new() { CatalogueItem = associatedService.CatalogueItem },
                new() { CatalogueItem = additionalService.CatalogueItem },
            };

            orderItems.ForEach(oi => order.OrderItems.Add(oi));

            order.LastUpdatedByUser = user;
            order.OrderingParty = orderingParty;
            order.Supplier = supplier;

            var model = new ViewOrderModel(order);

            model.OrderItems.Should().AllSatisfy(oi => oi.Framework.Should().Be(framework));
        }
    }
}
