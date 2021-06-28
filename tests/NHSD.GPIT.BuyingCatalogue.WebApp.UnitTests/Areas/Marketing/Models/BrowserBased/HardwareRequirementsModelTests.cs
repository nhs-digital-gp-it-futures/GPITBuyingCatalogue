using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    public static class HardwareRequirementsModelTests
    {
        [Fact]
        public static void Description_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(HardwareRequirementsModel)
                .GetProperty(nameof(HardwareRequirementsModel.Description))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(500);
        }

        [Theory]
        [AutoData]
        public static void IsComplete_DescriptionValid_ReturnsTrue(string description)
        {
            var model = new HardwareRequirementsModel { Description = description };

            model.IsComplete.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsComplete_DescriptionNotValid_ReturnsFalse(string invalid)
        {
            var model = new HardwareRequirementsModel { Description = invalid };

            model.IsComplete.Should().BeFalse();
        }
    }
}
