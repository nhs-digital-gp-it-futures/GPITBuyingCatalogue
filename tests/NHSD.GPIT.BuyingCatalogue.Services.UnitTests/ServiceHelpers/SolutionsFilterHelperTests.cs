using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ServiceHelpers
{
    public static class SolutionsFilterHelperTests
    {
        [Fact]
        public static void ParseCapabilityAndEpics_Empty()
        {
            var input = new Dictionary<int, string[]>() { };

            var result = SolutionsFilterHelper.ParseCapabilityAndEpicIds(input.ToFilterString());

            var expected = new Dictionary<int, string[]>() { };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityAndEpics_CapabilityId_Epics_Null()
        {
            var input = new Dictionary<int, string[]>() { { 1, null } };

            var result = SolutionsFilterHelper.ParseCapabilityAndEpicIds(input.ToFilterString());

            var expected = new Dictionary<int, string[]>() { { 1, Array.Empty<string>() } };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityAndEpics_CapabilityId_Epics_Empty()
        {
            var input = new Dictionary<int, string[]>() { { 1, Array.Empty<string>() } };

            var result = SolutionsFilterHelper.ParseCapabilityAndEpicIds(input.ToFilterString());

            var expected = new Dictionary<int, string[]>() { { 1, Array.Empty<string>() } };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityAndEpics_Capability_With_Epics()
        {
            var input = new Dictionary<int, string[]>()
            {
                { 1, new string[] { "Epic1", "Epic2" } },
                { 2, new string[] { "Epic1", "Epic3" } },
            };

            var result = SolutionsFilterHelper.ParseCapabilityAndEpicIds(input.ToFilterString());

            var expected = new Dictionary<int, string[]>()
            {
                { 1, new string[] { "Epic1", "Epic2" } },
                { 2, new string[] { "Epic1", "Epic3" } },
            };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseIntegrationAndTypeIds_Empty()
        {
            var input = new Dictionary<SupportedIntegrations, int[]>();

            var result = SolutionsFilterHelper.ParseIntegrationAndTypeIds(input.ToFilterString());

            result.Should().BeEquivalentTo(input);
        }

        [Fact]
        public static void ParseIntegrationAndTypeIds_IntegrationId_Types_Null()
        {
            var input = new Dictionary<SupportedIntegrations, int[]> { { SupportedIntegrations.Im1, null } };

            var result = SolutionsFilterHelper.ParseIntegrationAndTypeIds(input.ToFilterString());

            var expected = new Dictionary<SupportedIntegrations, int[]> { { SupportedIntegrations.Im1, [] } };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseIntegrationAndTypeIds_IntegrationId_Types_Empty()
        {
            var input = new Dictionary<SupportedIntegrations, int[]> { { SupportedIntegrations.Im1, [] } };

            var result = SolutionsFilterHelper.ParseIntegrationAndTypeIds(input.ToFilterString());

            result.Should().BeEquivalentTo(input);
        }

        [Fact]
        public static void ParseIntegrationAndTypeIds_Integration_With_Types()
        {
            var input = new Dictionary<SupportedIntegrations, int[]>
            {
                { SupportedIntegrations.Im1, [1, 2] },
                { SupportedIntegrations.GpConnect, [3, 4] },
            };

            var result = SolutionsFilterHelper.ParseIntegrationAndTypeIds(input.ToFilterString());

            result.Should().BeEquivalentTo(input);
        }

        [Fact]
        public static void ParseApplicationTypeIds_NullInput_GeneratesResults()
        {
            var result = SolutionsFilterHelper.ParseEnumFilter<ApplicationType>(null);

            result.Should().BeEquivalentTo(new List<ApplicationType>());
        }

        [Fact]
        public static void ParseApplicationTypeIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2";

            var result = SolutionsFilterHelper.ParseEnumFilter<ApplicationType>(input);

            var expected = new List<ApplicationType> { ApplicationType.BrowserBased, ApplicationType.Desktop, ApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6";

            var result = SolutionsFilterHelper.ParseEnumFilter<ApplicationType>(input);

            var expected = new List<ApplicationType> { ApplicationType.BrowserBased, ApplicationType.Desktop, ApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    ";

            var result = SolutionsFilterHelper.ParseEnumFilter<ApplicationType>(input);

            var expected = new List<ApplicationType> { ApplicationType.BrowserBased, ApplicationType.Desktop, ApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public static void ParseApplicationTypeIds_RandomInput_GeneratesResults(
            string input)
        {
            var result = SolutionsFilterHelper.ParseEnumFilter<ApplicationType>(input);

            var expected = new List<ApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseEnumFilter<ApplicationType>(input);

            var expected = new List<ApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2";

            var result = SolutionsFilterHelper.ParseEnumFilter<ApplicationType>(input);

            var expected = new List<ApplicationType> { ApplicationType.BrowserBased, ApplicationType.Desktop, ApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_NullInput_GeneratesResults()
        {
            var result = SolutionsFilterHelper.ParseEnumFilter<HostingType>(null);

            result.Should().BeEquivalentTo(new List<HostingType>());
        }

        [Fact]
        public static void ParseHostingTypeIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2.3";

            var result = SolutionsFilterHelper.ParseEnumFilter<HostingType>(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6.3";

            var result = SolutionsFilterHelper.ParseEnumFilter<HostingType>(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    .3";

            var result = SolutionsFilterHelper.ParseEnumFilter<HostingType>(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public static void ParseHostingTypeIds_RandomInput_GeneratesResults(
            string input)
        {
            var result = SolutionsFilterHelper.ParseEnumFilter<HostingType>(input);

            var expected = new List<ApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseEnumFilter<HostingType>(input);

            var expected = new List<ApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2.3";

            var result = SolutionsFilterHelper.ParseEnumFilter<HostingType>(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseSupportedIntegrationIds_NullInput_GeneratesResults()
        {
            var result = SolutionsFilterHelper.ParseEnumFilter<SupportedIntegrations>(null);

            result.Should().BeEquivalentTo(new List<SupportedIntegrations>());
        }

        [Fact]
        public static void ParseSelectedFilterIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2.3";

            var result = SolutionsFilterHelper.ParseSelectedFilterIds<SupportedIntegrations>(input);

            var expected = new List<SupportedIntegrations> { SupportedIntegrations.Im1, SupportedIntegrations.GpConnect, SupportedIntegrations.NhsApp };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseSelectedFilterIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6.3";

            var result = SolutionsFilterHelper.ParseSelectedFilterIds<SupportedIntegrations>(input);

            var expected = new List<SupportedIntegrations> { SupportedIntegrations.Im1, SupportedIntegrations.GpConnect, SupportedIntegrations.NhsApp };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseSelectedFilterIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    .3";

            var result = SolutionsFilterHelper.ParseSelectedFilterIds<SupportedIntegrations>(input);

            var expected = new List<SupportedIntegrations> { SupportedIntegrations.Im1, SupportedIntegrations.GpConnect, SupportedIntegrations.NhsApp };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseSelectedFilterIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseSelectedFilterIds<SupportedIntegrations>(input);

            var expected = new List<SupportedIntegrations>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseSelectedFilterIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2.3";

            var result = SolutionsFilterHelper.ParseSelectedFilterIds<SupportedIntegrations>(input);

            var expected = new List<SupportedIntegrations> { SupportedIntegrations.Im1, SupportedIntegrations.GpConnect, SupportedIntegrations.NhsApp };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseSelectedFilterIds_InvalidInput_ThrowsArgumentException()
        {
            var input = "invalid.enum.value";

            Action action = () => SolutionsFilterHelper.ParseSelectedFilterIds<SupportedIntegrations>(input);

            action.Should().Throw<ArgumentException>().WithMessage("Invalid filter format*");
        }
    }
}
