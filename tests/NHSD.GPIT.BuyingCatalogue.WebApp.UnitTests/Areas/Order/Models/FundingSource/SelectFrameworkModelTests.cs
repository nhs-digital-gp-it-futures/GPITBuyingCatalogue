using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.FundingSource
{
    public static class SelectFrameworkModelTests
    {
        [Theory]
        [MockAutoData]
        public static void SelectFrameworkModel_WithArguments_SetsCorrectly(
            EntityFramework.Ordering.Models.Order order,
            List<EntityFramework.Catalogue.Models.Framework> frameworks)
        {
            var model = new SelectFrameworkModel(order, frameworks);

            var expectedList = frameworks.Select(f => new SelectOption<string>(f.ShortName, f.Id));

            model.Title.Should().Be(SelectFrameworkModel.TitleText);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.SelectedFramework.Should().Be(order.SelectedFrameworkId);
            model.AssociatedServicesOnly.Should().Be(order.OrderType.AssociatedServicesOnly);
            model.Frameworks.Should().BeEquivalentTo(expectedList);
        }

        [Theory]
        [MockAutoData]
        public static void SelectFrameworkModel_SetFramework_ExpectedValues(
            EntityFramework.Catalogue.Models.Framework framework)
        {
            var list = new List<EntityFramework.Catalogue.Models.Framework>() { framework };

            var model = new SelectFrameworkModel();

            model.SetFrameworks(list);

            model.Frameworks.First().Value.Should().Be(framework.Id);
            model.Frameworks.First().Text.Should().Be(framework.ShortName);
        }
    }
}
