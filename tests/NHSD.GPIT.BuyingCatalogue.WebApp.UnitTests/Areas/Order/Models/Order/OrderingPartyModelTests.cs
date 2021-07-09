﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class OrderingPartyModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            Organisation organisation)
        {
            var model = new OrderingPartyModel(odsCode, order, organisation);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{order.CallOffId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Call-off Ordering Party information for {order.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.OrganisationName.Should().Be(organisation.Name);
            model.Address.Should().BeEquivalentTo(organisation.Address);

            // TODO: Contact
        }
    }
}
