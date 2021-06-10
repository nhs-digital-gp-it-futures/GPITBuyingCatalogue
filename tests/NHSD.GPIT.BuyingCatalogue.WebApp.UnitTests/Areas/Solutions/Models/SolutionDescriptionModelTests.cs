using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionDescriptionModelTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };
        
        [Test]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(SolutionDescriptionModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Test, AutoData]
        public static void HasDescription_ValidDescription_ReturnsTrue(SolutionDescriptionModel model)
        {
            model.Description.Should().NotBeNullOrWhiteSpace();

            var actual = model.HasDescription();

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void HasDescription_InvalidDescription_ReturnsFalse(string invalid)
        {
            var model = new SolutionDescriptionModel { Description = invalid };

            var actual = model.HasDescription();

            actual.Should().BeFalse();
        }
        
        [Test, AutoData]
        public static void HasSummary_ValidSummary_ReturnsTrue(SolutionDescriptionModel model)
        {
            model.Summary.Should().NotBeNullOrWhiteSpace();

            var actual = model.HasSummary();

            actual.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void HasSummary_InvalidSummary_ReturnsFalse(string invalid)
        {
            var model = new SolutionDescriptionModel { Summary = invalid };

            var actual = model.HasSummary();

            actual.Should().BeFalse();
        }
    }
}
