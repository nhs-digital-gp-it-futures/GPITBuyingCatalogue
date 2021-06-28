using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class IntegrationModelTests
    {
        [Theory]
        [AutoData]
        public static void Title_TablesMoreThanOne_ReturnsPluralTitle(IntegrationModel model)
        {
            model.Tables.Length.Should().BeGreaterThan(1);

            var actual = model.Title();

            actual.Should().Be($"{model.Name} integrations");
        }

        [Theory]
        [AutoData]
        public static void Title_OneTable_ReturnsSingularTitle(IntegrationModel model)
        {
            model.Tables = new[] { model.Tables[0] };

            var actual = model.Title();

            actual.Should().Be($"{model.Name} integration");
        }

        [Theory]
        [AutoData]
        public static void Title_NullTables_ReturnsSingularTitle(IntegrationModel model)
        {
            model.Tables = null;

            var actual = model.Title();

            actual.Should().Be($"{model.Name} integration");
        }
    }
}
