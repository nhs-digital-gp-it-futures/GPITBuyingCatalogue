using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class CommunityPharmacySolutionDisplayBaseModelTests
{
    [Fact]
    public static void Construct_NullCatalogueItem_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CommunityPharmacySolutionDisplayStub(null, new CatalogueItemContentStatus()));

        exception.ParamName.Should().Be("catalogueItem");
    }

    [Fact]
    public static void Construct_NullSolution_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new CommunityPharmacySolutionDisplayStub(new CatalogueItem(), new CatalogueItemContentStatus()));

        exception.ParamName.Should().Be("catalogueItem");
    }

    [Theory]
    [MockInlineAutoData(false)]
    [MockInlineAutoData(true)]
    public static void Construct_MapsSolutionAndSubPage(
        bool isSubPage,
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var model = new CommunityPharmacySolutionDisplayStub(solution.CatalogueItem, contentStatus, isSubPage);

        model.IsSubPage.Should().Be(isSubPage);
        model.SolutionId.Should().Be(solution.CatalogueItemId);
        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.PublicationStatus.Should().Be(solution.CatalogueItem.PublishedStatus);
        model.SupplierName.Should().Be(solution.CatalogueItem.Supplier.Name);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsCommunityPharmacyBreadcrumbs(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var model = new CommunityPharmacySolutionDisplayStub(solution.CatalogueItem, contentStatus);

        model.BreadcrumbItems.Select(x => x.Name).Should().Equal("Home", "Community pharmacy", "Pharmacy solutions");
        var solutionsBreadcrumb = model.BreadcrumbItems.Last();
        solutionsBreadcrumb.Action.Should().Be(nameof(CommunityPharmacyController.Solutions));
        solutionsBreadcrumb.Controller.Should().Be("CommunityPharmacy");
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsCommunityPharmacySectionsAndPagination(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var model = new CommunityPharmacySolutionDisplayStub(solution.CatalogueItem, contentStatus);

        model.Sections.Select(x => x.Action).Should().Equal(
            nameof(CommunityPharmacyController.Summary),
            nameof(CommunityPharmacyController.ListPrice),
            nameof(CommunityPharmacyController.Capabilities),
            nameof(CommunityPharmacyController.Standards),
            nameof(CommunityPharmacyController.Integrations),
            nameof(CommunityPharmacyController.ApplicationTypes),
            nameof(CommunityPharmacyController.HostingType),
            nameof(CommunityPharmacyController.DevelopmentPlans),
            nameof(CommunityPharmacyController.SupplierDetails));
    }

    [Theory]
    [MockInlineAutoData(PublicationStatus.Published, false, true, true)]
    [MockInlineAutoData(PublicationStatus.InRemediation, false, true, true)]
    [MockInlineAutoData(PublicationStatus.Suspended, false, false, false)]
    [MockInlineAutoData(PublicationStatus.Published, true, true, false)]
    public static void Construct_SetsNavigationVisibility(
        PublicationStatus publicationStatus,
        bool isSubPage,
        bool showSideNavigation,
        bool showPagination,
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.CatalogueItem.PublishedStatus = publicationStatus;

        var model = new CommunityPharmacySolutionDisplayStub(solution.CatalogueItem, contentStatus, isSubPage);

        model.ShowSideNavigation.Should().Be(showSideNavigation);
        model.ShowPagination.Should().Be(showPagination);
    }

    private sealed class CommunityPharmacySolutionDisplayStub : CommunityPharmacySolutionDisplayBaseModel
    {
        public CommunityPharmacySolutionDisplayStub(
            CatalogueItem catalogueItem,
            CatalogueItemContentStatus contentStatus,
            bool isSubPage = false)
            : base(catalogueItem, contentStatus, isSubPage)
        {
        }

        public override int Index => 4;
    }
}
