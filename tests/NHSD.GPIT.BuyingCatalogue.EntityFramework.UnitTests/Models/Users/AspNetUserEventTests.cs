using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Users;

public static class AspNetUserEventTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        int eventTypeId)
    {
        var model = new AspNetUserEvent(eventTypeId);

        model.EventTypeId.Should().Be(eventTypeId);
    }
}
