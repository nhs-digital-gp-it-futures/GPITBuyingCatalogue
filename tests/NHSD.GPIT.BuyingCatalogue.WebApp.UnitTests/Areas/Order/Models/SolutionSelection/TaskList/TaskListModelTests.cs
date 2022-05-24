using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.TaskList;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.TaskList
{
    public class TaskListModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithNullOrder_ThrowsException(
            string internalOrgId,
            CallOffId callOffId)
        {
            FluentActions
                .Invoking(() => new TaskListModel(internalOrgId, callOffId, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderItems.ElementAt(0).CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.ElementAt(1).CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            order.OrderItems.ElementAt(2).CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            var model = new TaskListModel(internalOrgId, callOffId, order);

            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
            model.CallOffId.Should().BeEquivalentTo(callOffId);
            model.AssociatedServicesOnly.Should().Be(order.AssociatedServicesOnly);
            model.CatalogueSolution.Should().BeEquivalentTo(order.OrderItems.ElementAt(0));
            model.AdditionalServices.Should().BeEquivalentTo(new[] { order.OrderItems.ElementAt(1) });
            model.AssociatedServices.Should().BeEquivalentTo(new[] { order.OrderItems.ElementAt(2) });
        }
    }
}
