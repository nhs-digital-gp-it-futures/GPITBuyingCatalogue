using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AdditionalInformationModelTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void AdditionalInformation_StringLengthAttribute_ExpectedValue()
        {
            typeof(AdditionalInformationModel)
                .GetProperty(nameof(AdditionalInformationModel.AdditionalInformation))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should().Be(500);
        }

        [Test]
        [AutoData]
        public static void IsComplete_AdditionalInformationHasValue_ReturnsTrue(string additionalInfo)
        {
            var model = new AdditionalInformationModel { AdditionalInformation = additionalInfo };

            model.IsComplete.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void IsComplete_AdditionalInformationInvalid_ReturnsFalse(string invalid)
        {
            var model = new AdditionalInformationModel { AdditionalInformation = invalid };

            model.IsComplete.Should().BeFalse();
        }
    }
}
