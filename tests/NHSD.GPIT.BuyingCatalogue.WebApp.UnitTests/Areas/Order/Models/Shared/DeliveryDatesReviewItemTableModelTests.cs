using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Shared
{
    public static class DeliveryDatesReviewItemTableModelTests
    {
        [Fact]
        public static void DefaultConstructor_SetsDefaults()
        {
            var model = new DeliveryDatesReviewItemTableModel();

            model.Recipients.Should().BeEmpty();
            model.ShowPlannedDeliveryDate.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string labelText,
            string editDatesUrl,
            DateTime deliveryDate)
        {
            var recipients = new List<(string OdsCode, string Name, DateTime? DeliveryDate)>
            {
                ("ODS1", "Recipient One", deliveryDate),
                ("ODS2", "Recipient Two", null),
            };

            var model = new DeliveryDatesReviewItemTableModel(labelText, editDatesUrl, recipients);

            model.LabelText.Should().Be(labelText);
            model.EditDatesUrl.Should().Be(editDatesUrl);
            model.Recipients.Should().BeEquivalentTo(recipients, opt => opt.WithStrictOrdering());
            model.ShowPlannedDeliveryDate.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void WithShowPlannedDeliveryDateFalse_PropertiesCorrectlySet(
            string labelText,
            string editDatesUrl,
            DateTime deliveryDate)
        {
            var recipients = new List<(string OdsCode, string Name, DateTime? DeliveryDate)>
            {
                ("ODS1", "Recipient One", deliveryDate),
            };

            var model = new DeliveryDatesReviewItemTableModel(
                labelText,
                editDatesUrl,
                recipients,
                showPlannedDeliveryDate: false);

            model.LabelText.Should().Be(labelText);
            model.EditDatesUrl.Should().Be(editDatesUrl);
            model.Recipients.Should().BeEquivalentTo(recipients);
            model.ShowPlannedDeliveryDate.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void WithNullRecipients_SetsRecipientsToEmpty(
            string labelText,
            string editDatesUrl)
        {
            var model = new DeliveryDatesReviewItemTableModel(labelText, editDatesUrl, null);

            model.Recipients.Should().BeEmpty();
        }
    }
}
