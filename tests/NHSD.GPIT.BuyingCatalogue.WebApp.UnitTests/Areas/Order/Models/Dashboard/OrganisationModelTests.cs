using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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

            model.BackLink.Should().Be("/");
            model.BackLinkText.Should().Be("Go back to homepage");
            model.Title.Should().Be(organisation.Name);
            model.OrganisationName.Should().Be(organisation.Name);
            model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
            model.CanActOnBehalf.Should().Be(user.GetSecondaryOrganisationInternalIdentifiers().Any());
            model.Orders.Should().BeEquivalentTo(allOrders);
        }

        [Theory]
        [CommonAutoData]
        public static void LinkName_Terminated_ReturnsExpectedLink(
            EntityFramework.Ordering.Models.Order order,
            OrganisationModel model)
        {
            order.Completed = DateTime.UtcNow;
            order.IsDeleted = false;
            order.IsTerminated = true;

            model.OrderIds = Enumerable.Empty<CallOffId>();

            model.LinkName(order).Should().Be("View");
        }

        [Theory]
        [CommonAutoData]
        public static void LinkName_CompletedNoSubsequentRevisions_ReturnsExpectedLink(
            EntityFramework.Ordering.Models.Order order,
            OrganisationModel model)
        {
            order.Completed = DateTime.UtcNow;
            order.IsDeleted = false;

            model.OrderIds = Enumerable.Empty<CallOffId>();

            model.LinkName(order).Should().Be("View");
        }

        [Theory]
        [CommonAutoData]
        public static void LinkName_InProgressNoSubsequentRevisions_ReturnsExpectedLink(
            EntityFramework.Ordering.Models.Order order,
            OrganisationModel model)
        {
            order.Completed = null;
            order.IsDeleted = false;

            model.OrderIds = Enumerable.Empty<CallOffId>();

            model.LinkName(order).Should().Be("Edit");
        }

        [Theory]
        [CommonAutoData]
        public static void LinkName_CompletedWithSubsequentRevisions_ReturnsExpectedLink(
            EntityFramework.Ordering.Models.Order order,
            OrganisationModel model)
        {
            order.Completed = DateTime.UtcNow;
            order.IsDeleted = false;

            model.OrderIds = Enumerable.Range(2, 6).Select(x => new CallOffId(order.OrderNumber, x));

            model.LinkName(order).Should().Be("View");
        }

        [Theory]
        [CommonAutoData]
        public static void LinkName_InProgressWithSubsequentRevisions_ReturnsExpectedLink(
            EntityFramework.Ordering.Models.Order order,
            OrganisationModel model)
        {
            order.Completed = null;
            order.IsDeleted = false;

            model.OrderIds = Enumerable.Range(2, 6).Select(x => new CallOffId(order.OrderNumber, x));

            model.LinkName(order).Should().Be("Edit");
        }

        [Theory]
        [CommonAutoData]
        public static void LinkName_CompletedAssociatedServiceOrder_ReturnsExpectedLink(
            EntityFramework.Ordering.Models.Order order,
            OrganisationModel model)
        {
            order.Completed = DateTime.UtcNow;
            order.IsDeleted = false;

            model.OrderIds = Enumerable.Empty<CallOffId>();

            model.LinkName(order).Should().Be("View");
        }

        [Theory]
        [CommonAutoData]
        public static void LinkName_InProgressAssociatedServiceOrder_ReturnsExpectedLink(
            EntityFramework.Ordering.Models.Order order,
            OrganisationModel model)
        {
            order.Completed = null;
            order.IsDeleted = false;

            model.OrderIds = Enumerable.Empty<CallOffId>();

            model.LinkName(order).Should().Be("Edit");
        }
    }
}
