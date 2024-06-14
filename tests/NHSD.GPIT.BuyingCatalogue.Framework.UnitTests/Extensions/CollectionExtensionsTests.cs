using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class CollectionExtensionsTests
    {
        [Theory]
        [MockAutoData]
        public static void AddRange_CorrectlyAddsRange(ICollection<string> expected)
        {
            var target = new HashSet<string>();

            target.AddRange(expected);

            target.Should().BeEquivalentTo(expected);
        }
    }
}
