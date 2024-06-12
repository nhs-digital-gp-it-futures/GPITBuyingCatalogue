using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class AssociatedServicesOnlyDetailsTests
{
    [Fact]
    public static void PracticeReorganisationNameAndCode_Empty()
    {
        var details = new AssociatedServicesOnlyDetails();

        details.PracticeReorganisationNameAndCode
            .Should()
            .BeEmpty();
    }

    [Theory]
    [MockAutoData]
    public static void When_NoRecipient_PracticeReorganisationNameAndCode_Empty(
        AssociatedServicesOnlyDetails details)
    {
        details.PracticeReorganisationRecipient = null;

        details.PracticeReorganisationNameAndCode
            .Should()
            .BeEmpty();
    }

    [Theory]
    [MockAutoData]
    public static void When_Recipient_And_Code_PracticeReorganisationNameAndCode_ExpectedResult(
        AssociatedServicesOnlyDetails details)
    {
        details.PracticeReorganisationNameAndCode
            .Should()
            .Be($"{details.PracticeReorganisationRecipient.Name} ({details.PracticeReorganisationOdsCode})");
    }
}
