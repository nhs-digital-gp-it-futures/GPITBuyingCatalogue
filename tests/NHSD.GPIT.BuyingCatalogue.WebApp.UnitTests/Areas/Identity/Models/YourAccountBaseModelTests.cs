using System.Linq;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Identity.Models
{
    public static class YourAccountBaseModelTests
    {
        [Fact]
        public static void Constructor_PropertiesCorrectlySet()
        {
            var model = new YourAccountModelStub();
            model.ShowBackToTop.Should().BeFalse();
            model.ShowBreadcrumb.Should().BeFalse();
            model.ShowPagination.Should().BeFalse();
            model.ShowSideNavigation.Should().BeTrue();
            model.Sections.Any().Should().BeTrue();
            model.BreadcrumbItems.Any().Should().BeFalse();
        }

        private sealed class YourAccountModelStub : YourAccountBaseModel
        {
            public YourAccountModelStub()
                : base()
            {
            }

            public override int Index => 0;
        }
    }
}
