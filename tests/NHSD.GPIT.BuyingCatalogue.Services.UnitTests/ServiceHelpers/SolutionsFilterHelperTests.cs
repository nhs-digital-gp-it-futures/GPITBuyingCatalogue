﻿using System.Collections.Generic;
using FluentAssertions;
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
    }
}
