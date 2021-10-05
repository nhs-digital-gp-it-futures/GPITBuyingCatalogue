using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard;
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
            model.OdsCode.Should().Be(organisation.OdsCode);
            model.CanActOnBehalf.Should().Be(user.GetSecondaryOdsCodes().Any());
            model.CompleteOrders.Should().BeEquivalentTo(allOrders.Where(o => !o.IsDeleted && o.OrderStatus == OrderStatus.Complete).ToList());
            model.InCompleteOrders.Should().BeEquivalentTo(allOrders.Where(o => !o.IsDeleted && o.OrderStatus == OrderStatus.Incomplete).ToList());
        }
    }
}
