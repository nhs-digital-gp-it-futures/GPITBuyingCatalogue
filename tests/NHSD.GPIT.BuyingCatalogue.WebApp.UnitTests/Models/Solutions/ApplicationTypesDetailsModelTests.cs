using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Solutions;

public static class ApplicationTypesDetailsModelTests
{
    [Fact]
    public static void Construct_NullApplicationTypeDetail_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => new ApplicationTypesDetailsModel(null));

        exception.ParamName.Should().Be("applicationTypeDetail");
    }

    [Theory]
    [InlineData(nameof(ApplicationTypesDetailsModel.ApplicationTypes))]
    [InlineData(nameof(ApplicationTypesDetailsModel.BrowserBasedApplication))]
    [InlineData(nameof(ApplicationTypesDetailsModel.NativeDesktopApplication))]
    [InlineData(nameof(ApplicationTypesDetailsModel.NativeMobileApplication))]
    public static void DescriptionListProperties_HaveExpectedUIHint(string propertyName)
    {
        typeof(ApplicationTypesDetailsModel).GetProperty(propertyName)
            .GetCustomAttribute<UIHintAttribute>().UIHint.Should().Be("DescriptionList");
    }

    [Fact]
    public static void Construct_NoApplicationTypes_ReturnsNoApplicationDetails()
    {
        var model = new ApplicationTypesDetailsModel(new ApplicationTypeDetail());

        model.BrowserBasedApplication.Should().BeNull();
        model.NativeDesktopApplication.Should().BeNull();
        model.NativeMobileApplication.Should().BeNull();
        model.ApplicationTypes.Items.Should().BeEmpty();
        model.HasApplicationType(ApplicationType.BrowserBased).Should().BeFalse();
    }

    [Theory]
    [InlineData(ApplicationType.BrowserBased)]
    [InlineData(ApplicationType.Desktop)]
    [InlineData(ApplicationType.MobileTablet)]
    public static void HasApplicationType_MatchesWithoutCaseSensitivity(ApplicationType applicationType)
    {
        var detail = new ApplicationTypeDetail
        {
            ApplicationTypes = new HashSet<string> { applicationType.EnumMemberName().ToUpperInvariant() },
        };

        var model = new ApplicationTypesDetailsModel(detail);

        model.ApplicationTypeDetail.Should().BeSameAs(detail);
        model.HasApplicationType(applicationType).Should().BeTrue();
    }

    [Theory]
    [InlineData(null, "Chrome")]
    [InlineData("", "Chrome")]
    [InlineData(" ", "Chrome")]
    [InlineData("100", "Chrome (Version 100)")]
    public static void Construct_SupportedBrowser_FormatsOptionalMinimumVersion(string version, string expected)
    {
        var detail = new ApplicationTypeDetail
        {
            ApplicationTypes = new() { ApplicationType.BrowserBased.EnumMemberName() },
            BrowsersSupported = new() { new() { BrowserName = "Chrome", MinimumBrowserVersion = version } },
        };

        var model = new ApplicationTypesDetailsModel(detail);

        model.BrowserBasedApplication.Items["Supported browser types"].List.Should().Equal(expected);
    }
}
