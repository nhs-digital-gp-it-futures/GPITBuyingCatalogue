using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ServiceHelpers
{
    public static class SolutionsFilterHelperTests
    {
        [Fact]
        public static void ParseCapabilityIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "5.6.hello.0";

            var result = SolutionsFilterHelper.ParseCapabilityIds(input);

            var expected = new List<int> { 5, 6, 0 };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "5.6. .0..    ";

            var result = SolutionsFilterHelper.ParseCapabilityIds(input);

            var expected = new List<int> { 5, 6, 0 };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityIds_RandomInput_GeneratesResults()
        {
            var input = "iogjhoiudfhjgouhouhagdf souihadsfgouihdsfg";

            var result = SolutionsFilterHelper.ParseCapabilityIds(input);

            var expected = new List<int>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseCapabilityIds(input);

            var expected = new List<int>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityIds_CorrectInput_GeneratesResults()
        {
            var input = "5.6.0";

            var result = SolutionsFilterHelper.ParseCapabilityIds(input);

            var expected = new List<int> { 5, 6, 0 };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseClientApplicationTypeIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2";

            var result = SolutionsFilterHelper.ParseClientApplicationTypeIds(input);

            var expected = new List<ClientApplicationType> { ClientApplicationType.BrowserBased, ClientApplicationType.Desktop, ClientApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseClientApplicationTypeIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6";

            var result = SolutionsFilterHelper.ParseClientApplicationTypeIds(input);

            var expected = new List<ClientApplicationType> { ClientApplicationType.BrowserBased, ClientApplicationType.Desktop, ClientApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseClientApplicationTypeIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    ";

            var result = SolutionsFilterHelper.ParseClientApplicationTypeIds(input);

            var expected = new List<ClientApplicationType> { ClientApplicationType.BrowserBased, ClientApplicationType.Desktop, ClientApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseClientApplicationTypeIds_RandomInput_GeneratesResults()
        {
            var input = "iogjhoiudfhjgouhouhagdf souihadsfgouihdsfg";

            var result = SolutionsFilterHelper.ParseClientApplicationTypeIds(input);

            var expected = new List<ClientApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseClientApplicationTypeIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseClientApplicationTypeIds(input);

            var expected = new List<ClientApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseClientApplicationTypeIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2";

            var result = SolutionsFilterHelper.ParseClientApplicationTypeIds(input);

            var expected = new List<ClientApplicationType> { ClientApplicationType.BrowserBased, ClientApplicationType.Desktop, ClientApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2.3";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6.3";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    .3";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_RandomInput_GeneratesResults()
        {
            var input = "iogjhoiudfhjgouhouhagdf souihadsfgouihdsfg";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<ClientApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<ClientApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2.3";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }
    }
}
