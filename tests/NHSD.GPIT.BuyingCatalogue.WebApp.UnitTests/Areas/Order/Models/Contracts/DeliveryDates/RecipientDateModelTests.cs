using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates
{
    public static class RecipientDateModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            OrderSublocationRecipient recipient,
            DateTime? deliveryDate,
            DateTime commencementDate)
        {
            var model = new RecipientDateModel(recipient, deliveryDate, commencementDate);

            model.OdsCode.Should().Be(recipient.RecipientOdsCode);
            model.Description.Should().Be(recipient.RecipientOdsOrganisation.Name);
            model.CommencementDate.Should().Be(commencementDate);
            model.Location.Should().Be(recipient.ParentSublocation.SublocationOrganisation.Name);

            model.Date.Should().Be(deliveryDate.Value.Date);
            model.Day.Should().Be($"{deliveryDate.Value.Day:00}");
            model.Month.Should().Be($"{deliveryDate.Value.Month:00}");
            model.Year.Should().Be($"{deliveryDate.Value.Year:0000}");
        }
    }
}
