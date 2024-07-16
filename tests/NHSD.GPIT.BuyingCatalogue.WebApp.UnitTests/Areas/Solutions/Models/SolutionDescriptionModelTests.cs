using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
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
        public static void SolutionDescriptionModel_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new SolutionDescriptionModel(null, new CatalogueItemContentStatus()));
            actual.ParamName.Should().Be("catalogueItem");
        }

        [Fact]
        public static void SolutionDescriptionModel_NullSolution_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(
                () => new SolutionDescriptionModel(
                    new CatalogueItem() { Solution = null },
                    new CatalogueItemContentStatus()));
            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [MockAutoData]
        public static void FrameworkTitle_FrameworksMoreThanOne_ReturnsPlural(SolutionDescriptionModel model)
        {
            model.Frameworks.Count.Should().BeGreaterThan(1);

            var actual = model.FrameworkTitle();

            actual.Should().Be("Frameworks");
        }

        [Theory]
        [MockAutoData]
        public static void FrameworkTitle_OneFramework_ReturnsSingle(
            EntityFramework.Catalogue.Models.Framework framework)
        {
            var model = new SolutionDescriptionModel
            {
                Frameworks = new List<EntityFramework.Catalogue.Models.Framework>() { framework },
            };

            var actual = model.FrameworkTitle();

            actual.Should().Be("Framework");
        }

        [Fact]
        public static void FrameworkTitle_NoFramework_ReturnsSingle()
        {
            var model = new SolutionDescriptionModel { Frameworks = Enumerable.Empty<EntityFramework.Catalogue.Models.Framework>().ToList() };

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
        [MockAutoData]
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
        [MockAutoData]
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
        [MockAutoData]
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
