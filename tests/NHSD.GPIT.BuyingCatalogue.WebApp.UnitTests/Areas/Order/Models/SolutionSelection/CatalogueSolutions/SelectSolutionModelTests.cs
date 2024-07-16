using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.CatalogueSolutions
{
    public static class SelectSolutionModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> solutions,
            List<CatalogueItem> additionalServices)
        {
            var model = new SelectSolutionModel(order, solutions, additionalServices);

            model.OrderType.Should().Be(order.OrderType);
            model.CallOffId.Should().Be(order.CallOffId);
            model.SupplierName.Should().Be(order.Supplier.Name);

            foreach (var solution in solutions)
            {
                model.CatalogueSolutions.Should().Contain(x => x.Text == solution.Name && x.Value == $"{solution.Id}");
            }

            foreach (var service in additionalServices)
            {
                model.AdditionalServices.Should().Contain(x => x.Description == service.Name && x.CatalogueItemId == service.Id);
            }
        }

        [Theory]
        [MockAutoData]
        public static void GetAdditionalServicesIdsForSelectedCatalogueSolution_ReturnsExpectedResult(
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> solutions,
            List<CatalogueItem> additionalServices)
        {
            var catalogueItemId = new CatalogueItemId(1, "abc");

            for (var i = 0; i < additionalServices.Count; i++)
            {
                additionalServices[i].Id = CatalogueItemId.ParseExact($"{catalogueItemId}{i:000}");
            }

            var model = new SelectSolutionModel(order, solutions, additionalServices)
            {
                SelectedCatalogueSolutionId = $"{catalogueItemId}",
            };

            model.AdditionalServices.ForEach(x => x.IsSelected = true);

            var expected = model.AdditionalServices.Select(x => x.CatalogueItemId);
            var actual = model.GetAdditionalServicesIdsForSelectedCatalogueSolution();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
