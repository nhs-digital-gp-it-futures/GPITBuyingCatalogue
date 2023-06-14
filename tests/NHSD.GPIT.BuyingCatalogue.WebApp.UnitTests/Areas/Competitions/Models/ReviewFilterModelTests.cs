using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models;

public static class ReviewFilterModelTests
{
    public static IEnumerable<object[]> HasEpicsTestData => new[]
    {
        new object[]
        {
            new List<KeyValuePair<string, List<string>>> { new("Capability", new List<string> { "Epic" }), },
            true,
        },
        new object[] { Enumerable.Empty<KeyValuePair<string, List<string>>>().ToList(), false },
    };

    public static IEnumerable<object[]> HasHostingTypesTestData => new[]
    {
        new object[] { new List<HostingType> { HostingType.Hybrid }, true, },
        new object[] { Enumerable.Empty<HostingType>().ToList(), false, },
    };

    public static IEnumerable<object[]> HasApplicationTypesTestData => new[]
    {
        new object[] { new List<ApplicationType> { ApplicationType.Desktop }, true, },
        new object[] { Enumerable.Empty<ApplicationType>().ToList(), false, },
    };

    public static IEnumerable<object[]> HasAdditionalFiltersTestData => new[]
    {
        new object[] { new FilterDetailsModel(), false },
        new object[] { new FilterDetailsModel { FrameworkName = "Framework" }, true },
        new object[]
        {
            new FilterDetailsModel
            {
                ApplicationTypes = new List<ApplicationType> { ApplicationType.Desktop },
            },
            true,
        },
        new object[]
        {
            new FilterDetailsModel { HostingTypes = new List<HostingType> { HostingType.Hybrid }, }, true,
        },
        new object[]
        {
            new FilterDetailsModel
            {
                FrameworkName = "Framework",
                ApplicationTypes = new List<ApplicationType> { ApplicationType.Desktop },
                HostingTypes = new List<HostingType> { HostingType.Hybrid },
            },
            true,
        },
    };

    [Theory]
    [CommonAutoData]
    public static void Construct_With_FilterDetails_SetsProperty(
        FilterDetailsModel filterDetailsModel)
    {
        var model = new ReviewFilterModel(filterDetailsModel);

        model.FilterDetails.Should().Be(filterDetailsModel);
        model.FilterIds.Should().BeNull();
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_With_FilterDetails_And_FilterIds_SetsProperties(
        FilterDetailsModel filterDetailsModel,
        FilterIdsModel filterIdsModel)
    {
        var model = new ReviewFilterModel(filterDetailsModel, filterIdsModel);

        model.FilterDetails.Should().Be(filterDetailsModel);
        model.FilterIds.Should().Be(filterIdsModel);
    }

    [Theory]
    [CommonInlineAutoData("Framework", true)]
    [CommonInlineAutoData(null, false)]
    public static void HasFramework_ReturnsExpected(
        string framework,
        bool expected,
        FilterDetailsModel filterDetailsModel)
    {
        filterDetailsModel.FrameworkName = framework;

        var model = new ReviewFilterModel(filterDetailsModel);

        model.HasFramework().Should().Be(expected);
    }

    [Theory]
    [CommonMemberAutoData(nameof(HasEpicsTestData))]
    public static void HasEpics_ReturnsExpected(
        List<KeyValuePair<string, List<string>>> capabilities,
        bool expected,
        FilterDetailsModel filterDetailsModel)
    {
        filterDetailsModel.Capabilities = capabilities;

        var model = new ReviewFilterModel(filterDetailsModel);

        model.HasEpics().Should().Be(expected);
    }

    [Theory]
    [CommonMemberAutoData(nameof(HasHostingTypesTestData))]
    public static void HasHostingTypes_ReturnsExpected(
        List<HostingType> hostingTypes,
        bool expected,
        FilterDetailsModel filterDetailsModel)
    {
        filterDetailsModel.HostingTypes = hostingTypes;

        var model = new ReviewFilterModel(filterDetailsModel);

        model.HasHostingTypes().Should().Be(expected);
    }

    [Theory]
    [CommonMemberAutoData(nameof(HasApplicationTypesTestData))]
    public static void HasApplicationTypes_ReturnsExpected(
        List<ApplicationType> clientApplicationTypes,
        bool expected,
        FilterDetailsModel filterDetailsModel)
    {
        filterDetailsModel.ApplicationTypes = clientApplicationTypes;

        var model = new ReviewFilterModel(filterDetailsModel);

        model.HasApplicationTypes().Should().Be(expected);
    }

    [Theory]
    [CommonMemberAutoData(nameof(HasAdditionalFiltersTestData))]
    public static void HasAdditionalFilters_ReturnsExpected(
        FilterDetailsModel filterDetailsModel,
        bool expected)
    {
        var model = new ReviewFilterModel(filterDetailsModel);

        model.HasAdditionalFilters().Should().Be(expected);
    }
}
