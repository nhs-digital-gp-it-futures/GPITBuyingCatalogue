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
    internal static class HardwareRequirementsModelTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };
        
        [Test]
        public static void Description_StringLengthAttribute_ExpectedMaxLength()
        {
            typeof(HardwareRequirementsModel)
                .GetProperty(nameof(HardwareRequirementsModel.Description))
                .GetCustomAttribute<StringLengthAttribute>()
                .MaximumLength.Should()
                .Be(500);
        }

        [AutoData]
        [Test]
        public static void IsComplete_DescriptionValid_ReturnsTrue(string description)
        {
            var model = new HardwareRequirementsModel { Description = description };

            model.IsComplete.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void IsComplete_DescriptionNotValid_ReturnsFalse(string invalid)
        {
            var model = new HardwareRequirementsModel { Description = invalid };

            model.IsComplete.Should().BeFalse();
        }
    }
}
