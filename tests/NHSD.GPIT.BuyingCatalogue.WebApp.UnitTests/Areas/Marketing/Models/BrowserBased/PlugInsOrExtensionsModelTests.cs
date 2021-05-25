using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class PlugInsOrExtensionsModelTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void AdditionalInformation_StringLengthAttribute_ExpectedValue()
        {
            typeof(AdditionalInformationModel)
                .GetProperty(nameof(PlugInsOrExtensionsModel.AdditionalInformation))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should().Be(500);
        }

        [Test]
        public static void IsComplete_PluginsRequiredValue_ReturnsTrue()
        {
            var model = new PlugInsOrExtensionsModel { PlugInsRequired = "some-value" };

            model.IsComplete.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void IsComplete_PlugInsRequiredInvalid_ReturnsFalse(string invalid)
        {
            var model = new PlugInsOrExtensionsModel { PlugInsRequired = invalid };

            model.IsComplete.Should().BeFalse();
        }
    }
}
