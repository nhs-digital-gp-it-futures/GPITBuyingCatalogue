using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    public static class PlugInsOrExtensionsModelTests
    {
        [Fact]
        public static void AdditionalInformation_StringLengthAttribute_ExpectedValue()
        {
            typeof(AdditionalInformationModel)
                .GetProperty(nameof(PlugInsOrExtensionsModel.AdditionalInformation))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should().Be(500);
        }

        [Fact]
        public static void IsComplete_PluginsRequiredValue_ReturnsTrue()
        {
            var model = new PlugInsOrExtensionsModel { PlugInsRequired = "some-value" };

            model.IsComplete.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsComplete_PlugInsRequiredInvalid_ReturnsFalse(string invalid)
        {
            var model = new PlugInsOrExtensionsModel { PlugInsRequired = invalid };

            model.IsComplete.Should().BeFalse();
        }
    }
}
