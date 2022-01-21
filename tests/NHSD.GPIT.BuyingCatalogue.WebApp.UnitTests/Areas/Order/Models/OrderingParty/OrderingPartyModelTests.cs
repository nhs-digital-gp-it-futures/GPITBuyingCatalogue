﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderingParty;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.OrderingParty
{
    public static class OrderingPartyModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new OrderingPartyModel(odsCode, order);

            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(order.CallOffId.ToString());
            model.Contact.FirstName.Should().Be(order.OrderingPartyContact.FirstName);
            model.Contact.LastName.Should().Be(order.OrderingPartyContact.LastName);
            model.Contact.EmailAddress.Should().Be(order.OrderingPartyContact.Email);
            model.Contact.TelephoneNumber.Should().Be(order.OrderingPartyContact.Phone);
        }
    }
}
