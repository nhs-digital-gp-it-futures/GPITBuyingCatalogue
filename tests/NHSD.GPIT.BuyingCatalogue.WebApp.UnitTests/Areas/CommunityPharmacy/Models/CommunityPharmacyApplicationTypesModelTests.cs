using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class CommunityPharmacyApplicationTypesModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_MapsApplicationTypes(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.ApplicationTypeDetail = new ApplicationTypeDetail
        {
            ApplicationTypes = new() { ApplicationType.BrowserBased.Name() },
            BrowsersSupported = new() { new() { BrowserName = "Chrome", MinimumBrowserVersion = "100" } },
        };

        var model = new CommunityPharmacyApplicationTypesModel(solution.CatalogueItem, contentStatus);

        model.ApplicationTypeDetail.Should().BeSameAs(solution.ApplicationTypeDetail);
        model.ApplicationTypes.Items.Should().ContainKey("Browser-based application");
        model.BrowserBasedApplication.Items["Supported browser types"].List
            .Should().Equal("Chrome (Version 100)");
        model.NativeDesktopApplication.Should().BeNull();
        model.NativeMobileApplication.Should().BeNull();
        model.SelectedSection.Should().Be("Application type");
        model.PaginationFooter.FullWidth.Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_NoApplicationTypeDetail_ReturnsEmptyApplicationTypes(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.ApplicationTypeDetail = null;

        var model = new CommunityPharmacyApplicationTypesModel(solution.CatalogueItem, contentStatus);

        model.Should().NotBeNull();
        model.ApplicationTypes.Items.Should().BeEmpty();
        model.BrowserBasedApplication.Should().BeNull();
        model.NativeDesktopApplication.Should().BeNull();
        model.NativeMobileApplication.Should().BeNull();
    }
}
