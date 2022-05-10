using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FrameworksAdded_ShouldAddToList(EntityFramework.Catalogue.Models.Framework framework)
        {
            var expectedCount = 10;

            var frameworks = new List<KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>>
            {
                new KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>(framework, expectedCount),
            };

            var model = new SolutionsModel(frameworks);

            model.FrameworkFilters.Count.Should().Be(1);
            model.FrameworkFilters.First().FrameworkId.Should().Be(framework.Id);
            model.FrameworkFilters.First().Count.Should().Be(expectedCount);
            model.FrameworkFilters.First().FrameworkFullName.Should().Be($"{framework.ShortName} framework");
        }

        [Fact]
        public static void FrameworksAdded_All_ShouldEndWithFrameworks()
        {
            var expectedCount = 10;

            var framework = new EntityFramework.Catalogue.Models.Framework { Id = "All", ShortName = "All" };

            var frameworks = new List<KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>>
            {
                new KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>(framework, expectedCount),
            };

            var model = new SolutionsModel(frameworks);

            model.FrameworkFilters.Count.Should().Be(1);
            model.FrameworkFilters.First().FrameworkId.Should().Be(framework.Id);
            model.FrameworkFilters.First().Count.Should().Be(expectedCount);
            model.FrameworkFilters.First().FrameworkFullName.Should().Be($"{framework.ShortName} frameworks");
        }
    }
}
