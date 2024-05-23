using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Dashboard;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Dashboard
{
    public static class OrganisationModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            Organisation organisation,
            List<Organisation> organisations,
            IList<EntityFramework.Ordering.Models.Order> allOrders)
        {
            var model = new OrganisationModel(organisation, organisations, allOrders);

            model.Title.Should().Be(organisation.Name);
            model.OrganisationName.Should().Be(organisation.Name);
            model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
            model.CanActOnBehalf.Should().Be(organisations.Count != 0);
            model.Orders.Should().BeEquivalentTo(allOrders);
        }

        [Theory]
        [MockInlineAutoData(OrderStatus.Terminated, NhsTagsTagHelper.TagColour.Red)]
        [MockInlineAutoData(OrderStatus.Completed, NhsTagsTagHelper.TagColour.Green)]
        [MockInlineAutoData(OrderStatus.InProgress, NhsTagsTagHelper.TagColour.Blue)]
        public static void TagColour_WithOrderStatus_ReturnsExpected(
            OrderStatus orderStatus,
            NhsTagsTagHelper.TagColour tagColour,
            OrganisationModel model) => model.TagColour(orderStatus).Should().Be(tagColour);
    }
}
