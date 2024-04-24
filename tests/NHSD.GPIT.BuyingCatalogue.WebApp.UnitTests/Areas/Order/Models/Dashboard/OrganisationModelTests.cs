using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Dashboard;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Dashboard
{
    public static class OrganisationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            Organisation organisation,
            ClaimsPrincipal user,
            IList<EntityFramework.Ordering.Models.Order> allOrders)
        {
            var model = new OrganisationModel(organisation, user, allOrders);

            model.Title.Should().Be(organisation.Name);
            model.OrganisationName.Should().Be(organisation.Name);
            model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
            model.CanActOnBehalf.Should().Be(user.GetSecondaryOrganisationInternalIdentifiers().Any());
            model.Orders.Should().BeEquivalentTo(allOrders);
        }

        [Theory]
        [CommonInlineAutoData(OrderStatus.Terminated, NhsTagsTagHelper.TagColour.Red)]
        [CommonInlineAutoData(OrderStatus.Completed, NhsTagsTagHelper.TagColour.Green)]
        [CommonInlineAutoData(OrderStatus.InProgress, NhsTagsTagHelper.TagColour.Blue)]
        public static void TagColour_WithOrderStatus_ReturnsExpected(
            OrderStatus orderStatus,
            NhsTagsTagHelper.TagColour tagColour,
            OrganisationModel model) => model.TagColour(orderStatus).Should().Be(tagColour);
    }
}
