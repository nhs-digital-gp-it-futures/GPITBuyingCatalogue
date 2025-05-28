using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Shared;

public static class SublocationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Constructor_SetsPropertiesAsExpected(
        string name,
        List<ServiceRecipientModel> serviceRecipients)
    {
        var model = new SublocationModel(
            name,
            serviceRecipients);

        model.Name.Should().Be(name);
        model.ServiceRecipients.Should().BeEquivalentTo(serviceRecipients);
    }

    [Theory]
    [MockInlineAutoData(true)]
    [MockInlineAutoData(false)]
    public static void AllRecipientsSelected_SetsPropertiesAsExpected(
        bool selected,
        string name,
        List<ServiceRecipientModel> serviceRecipients)
    {
        serviceRecipients.ForEach(x => x.Selected = selected);
        var model = new SublocationModel(
            name,
            serviceRecipients);

        model.Name.Should().Be(name);
        model.ServiceRecipients.Should().BeEquivalentTo(serviceRecipients);
        model.AllRecipientsSelected.Should().Be(selected);
    }
}
