using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionDescriptionModelTests
    {
        [Fact]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(SolutionDescriptionModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Fact]
        public static void Frameworks_UIHintAttribute_ExpectedHint()
        {
            typeof(SolutionDescriptionModel)
                .GetProperty(nameof(SolutionDescriptionModel.Frameworks))
                .GetCustomAttribute<UIHintAttribute>()
                .UIHint.Should()
                .Be("TableListCell");
        }

        [Theory]
        [CommonAutoData]
        public static void FrameworkTitle_FrameworksMoreThanOne_ReturnsPlural(SolutionDescriptionModel model)
        {
            model.Frameworks.Length.Should().BeGreaterThan(1);

            var actual = model.FrameworkTitle();

            actual.Should().Be("Frameworks");
        }

        [Theory]
        [CommonAutoData]
        public static void FrameworkTitle_OneFramework_ReturnsSingle(string framework)
        {
            var model = new SolutionDescriptionModel { Frameworks = new[] { framework } };

            var actual = model.FrameworkTitle();

            actual.Should().Be("Framework");
        }

        [Fact]
        public static void FrameworkTitle_NoFramework_ReturnsSingle()
        {
            var model = new SolutionDescriptionModel { Frameworks = System.Array.Empty<string>() };

            var actual = model.FrameworkTitle();

            actual.Should().Be("Framework");
        }

        [Fact]
        public static void FrameworkTitle_NullFramework_ReturnsSingle()
        {
            var model = new SolutionDescriptionModel { Frameworks = null };

            var actual = model.FrameworkTitle();

            actual.Should().Be("Framework");
        }

        [Theory]
        [CommonAutoData]
        public static void HasDescription_ValidDescription_ReturnsTrue(SolutionDescriptionModel model)
        {
            model.Description.Should().NotBeNullOrWhiteSpace();

            var actual = model.HasDescription();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasDescription_InvalidDescription_ReturnsFalse(string invalid)
        {
            var model = new SolutionDescriptionModel { Description = invalid };

            var actual = model.HasDescription();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasSummary_ValidSummary_ReturnsTrue(SolutionDescriptionModel model)
        {
            model.Summary.Should().NotBeNullOrWhiteSpace();

            var actual = model.HasSummary();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasSummary_InvalidSummary_ReturnsFalse(string invalid)
        {
            var model = new SolutionDescriptionModel { Summary = invalid };

            var actual = model.HasSummary();

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void HasAboutUrl_ValidUrl_ReturnsTrue(SolutionDescriptionModel model)
        {
            model.AboutUrl.Should().NotBeNullOrWhiteSpace();

            var actual = model.HasAboutUrl();

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void HasAboutUrl_InvalidUrl_ReturnsFalse(string invalid)
        {
            var model = new SolutionDescriptionModel { AboutUrl = invalid };

            var actual = model.HasAboutUrl();

            actual.Should().BeFalse();
        }
    }
}
