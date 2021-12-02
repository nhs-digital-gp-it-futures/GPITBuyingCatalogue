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
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DevelopmentPlanModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }
    }
}
