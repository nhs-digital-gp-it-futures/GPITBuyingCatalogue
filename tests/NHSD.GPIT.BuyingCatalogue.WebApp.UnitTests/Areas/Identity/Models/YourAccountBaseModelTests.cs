using System;
using System.Linq;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.YourAccount;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Models
{
    public static class YourAccountBaseModelTests
    {
        [Theory]
        [InlineData(typeof(YourAccountModel))]
        public static void ChildClasses_InheritFrom_SolutionDisplayBaseModel(Type childType)
        {
            childType
            .Should()
            .BeAssignableTo<YourAccountBaseModel>();
        }

        [Fact]
        public static void Constructor_PropertiesCorrectlySet()
        {
            var model = new Mock<YourAccountBaseModel>();
            model.Object.ShowBackToTop.Should().BeFalse();
            model.Object.ShowBreadcrumb.Should().BeFalse();
            model.Object.ShowPagination.Should().BeFalse();
            model.Object.ShowSideNavigation.Should().BeTrue();
            model.Object.Sections.Any().Should().BeTrue();
            model.Object.BreadcrumbItems.Any().Should().BeFalse();
        }
    }
}
