using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DevelopmentPlansModels
{
    public static class DevelopmentPlanModelTests
    {
        [Fact]
        public static void Link_Should_BeCorrectlyDecorated()
        {
            var propertyInfo = typeof(DevelopmentPlanModel)
                .GetProperty(nameof(DevelopmentPlanModel.Link), BindingFlags.Instance | BindingFlags.Public);

            propertyInfo
                .Should()
                .BeDecoratedWith<StringLengthAttribute>(s => s.MaximumLength == 1000);
            propertyInfo
                .Should()
                .BeDecoratedWith<UrlAttribute>();
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DevelopmentPlanModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [AutoData]
        public static void Status_LinkAdded_ReturnsCompleted(string link)
        {
            var model = new DevelopmentPlanModel { Link = link };

            var actual = model.Status();

            actual.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void Status_LinkInvalid_ReturnsOptional(string invalid)
        {
            var model = new DevelopmentPlanModel { Link = invalid };

            var actual = model.Status();

            actual.Should().Be(TaskProgress.Optional);
        }
    }
}
