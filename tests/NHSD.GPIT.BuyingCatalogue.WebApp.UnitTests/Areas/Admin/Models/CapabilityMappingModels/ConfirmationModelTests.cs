using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsConfirmationBanner;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.CapabilityMappingModels;

public static class ConfirmationModelTests
{
    public static IEnumerable<object[]> ConfirmationModelTestData => new[]
    {
        new object[]
        {
            true,
            NhsConfirmationBannerModel.BannerColour.Blue,
            "Capabilities and Epics successfully updated",
            "The updated Capabilities and Epics have been successfully mapped to the solutions and services on the Buying Catalogue.",
        },
        new object[]
        {
            false,
            NhsConfirmationBannerModel.BannerColour.Grey,
            "Capabilities and Epics update failed",
            "The Capabilities and Epics have not been mapped to the solutions and services on the Buying Catalogue. Go back to your admin homepage and try again.",
        },
    };

    [Theory]
    [MockMemberAutoData(nameof(ConfirmationModelTestData))]
    public static void Construct_SetsPropertiesAsExpected(
        bool isSuccessful,
        NhsConfirmationBannerModel.BannerColour bannerColour,
        string expectedTitle,
        string expectedContent)
    {
        var model = new ConfirmationModel(isSuccessful);

        model.Colour.Should().Be(bannerColour);
        model.Title.Should().Be(expectedTitle);
        model.Content.Should().Be(expectedContent);
    }
}
