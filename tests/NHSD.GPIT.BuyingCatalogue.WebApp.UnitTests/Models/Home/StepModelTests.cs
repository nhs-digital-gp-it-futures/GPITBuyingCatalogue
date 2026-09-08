using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Home
{
    public static class StepModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Construct_SetsPropertiesAsExpected(
        string title,
        string text,
        int number)
        {
            var model = new StepModel(title, text, number);

            model.Title.Should().Be(title);
            model.HelpText.Should().Be(text);
            model.Number.Should().Be(number);
        }
    }
}
