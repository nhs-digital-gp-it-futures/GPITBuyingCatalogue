using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.DashboardModels;

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

    public static IEnumerable<object[]> HasHasInteroperabilityIntegrationTypesTestData => new[]
    {
        new object[] { new List<SupportedIntegrations> { SupportedIntegrations.Im1 }, true, },
        new object[] { Enumerable.Empty<SupportedIntegrations>().ToList(), false, },
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
    [MockAutoData]
    public static void Construct_With_FilterDetails_SetsProperty(
        FilterDetailsModel filterDetailsModel)
    {
        var model = new ReviewFilterModel(filterDetailsModel);

        model.FilterDetails.Should().Be(filterDetailsModel);
        model.FilterIds.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_With_FilterDetails_And_FilterIds_SetsProperties(
        FilterDetailsModel filterDetailsModel,
        FilterIdsModel filterIdsModel)
    {
        var model = new ReviewFilterModel(filterDetailsModel, filterIdsModel);

        model.FilterDetails.Should().Be(filterDetailsModel);
        model.FilterIds.Should().Be(filterIdsModel);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_ExpectedModel(
        FilterDetailsModel filterDetails,
        string internalOrgId,
        List<CatalogueItem> filterResults,
        Solution solution,
        bool inCompetition,
        FilterIdsModel filterIds)
    {
        filterResults.ForEach(x => x.Solution = solution);
        filterIds.FrameworkId = null;

        var result = new ReviewFilterModel(filterDetails, internalOrgId, filterResults, inCompetition, filterIds);

        result.ResultsCount.Should().Be(filterResults.Count);
        result.InCompetition.Should().Be(inCompetition);

        var groupedResults = filterResults
            .SelectMany(x => x.Solution.FrameworkSolutions)
            .GroupBy(x => (x.Framework.ShortName, x.Framework.Id));
        result.ResultsForFrameworks.Count.Should().Be(groupedResults.Count());
    }

    [Theory]
    [MockInlineAutoData("Framework", true)]
    [MockInlineAutoData(null, false)]
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
    [MockMemberAutoData(nameof(HasEpicsTestData))]
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
    [MockMemberAutoData(nameof(HasHostingTypesTestData))]
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
    [MockMemberAutoData(nameof(HasApplicationTypesTestData))]
    public static void HasApplicationTypes_ReturnsExpected(
        List<ApplicationType> applicationTypes,
        bool expected,
        FilterDetailsModel filterDetailsModel)
    {
        filterDetailsModel.ApplicationTypes = applicationTypes;

        var model = new ReviewFilterModel(filterDetailsModel);

        model.HasApplicationTypes().Should().Be(expected);
    }

    [Theory]
    [MockMemberAutoData(nameof(HasHasInteroperabilityIntegrationTypesTestData))]
    public static void HasInteroperabilityIntegrationTypes_ReturnsExpected(
        List<SupportedIntegrations> interopIntegrationTypes,
        bool expected,
        FilterDetailsModel filterDetailsModel)
    {
        filterDetailsModel.InteropIntegrationTypes = interopIntegrationTypes;

        var model = new ReviewFilterModel(filterDetailsModel);

        model.HasInteroperabilityIntegrationTypes().Should().Be(expected);
    }

    [Theory]
    [MockMemberAutoData(nameof(HasAdditionalFiltersTestData))]
    public static void HasAdditionalFilters_ReturnsExpected(
        FilterDetailsModel filterDetailsModel,
        bool expected)
    {
        var model = new ReviewFilterModel(filterDetailsModel);

        model.HasAdditionalFilters().Should().Be(expected);
    }
}
