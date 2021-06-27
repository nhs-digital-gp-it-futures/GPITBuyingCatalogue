using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    public static class AdditionalInformationModelTests
    {
        [Fact]
        public static void AdditionalInformation_StringLengthAttribute_ExpectedValue()
        {
            typeof(AdditionalInformationModel)
                .GetProperty(nameof(AdditionalInformationModel.AdditionalInformation))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should().Be(500);
        }

        [Theory]
        [AutoData]
        public static void IsComplete_AdditionalInformationHasValue_ReturnsTrue(string additionalInfo)
        {
            var model = new AdditionalInformationModel { AdditionalInformation = additionalInfo };

            model.IsComplete.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsComplete_AdditionalInformationInvalid_ReturnsFalse(string invalid)
        {
            var model = new AdditionalInformationModel { AdditionalInformation = invalid };

            model.IsComplete.Should().BeFalse();
        }
    }
}
