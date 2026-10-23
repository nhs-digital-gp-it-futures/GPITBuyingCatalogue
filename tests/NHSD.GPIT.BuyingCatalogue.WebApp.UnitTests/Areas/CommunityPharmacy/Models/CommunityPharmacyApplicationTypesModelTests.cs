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
    public static void Construct_MapsApplicationTypesAndSetsFullWidthFooter(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.ApplicationTypeDetail = new ApplicationTypeDetail
        {
            ApplicationTypes = new() { ApplicationType.BrowserBased.EnumMemberName() },
            BrowsersSupported = new() { new() { BrowserName = "Chrome", MinimumBrowserVersion = "100" } },
        };

        var model = new CommunityPharmacyApplicationTypesModel(solution.CatalogueItem, contentStatus);

        model.ApplicationTypesDetails.ApplicationTypeDetail.Should().BeSameAs(solution.ApplicationTypeDetail);
        model.ApplicationTypesDetails.ApplicationTypes.Items.Should().ContainKey("Browser-based application");
        model.ApplicationTypesDetails.BrowserBasedApplication.Items["Supported browser types"].List
            .Should().Equal("Chrome (Version 100)");
        model.ApplicationTypesDetails.NativeDesktopApplication.Should().BeNull();
        model.ApplicationTypesDetails.NativeMobileApplication.Should().BeNull();
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

        model.ApplicationTypesDetails.Should().NotBeNull();
        model.ApplicationTypesDetails.ApplicationTypes.Items.Should().BeEmpty();
        model.ApplicationTypesDetails.BrowserBasedApplication.Should().BeNull();
        model.ApplicationTypesDetails.NativeDesktopApplication.Should().BeNull();
        model.ApplicationTypesDetails.NativeMobileApplication.Should().BeNull();
    }
}
