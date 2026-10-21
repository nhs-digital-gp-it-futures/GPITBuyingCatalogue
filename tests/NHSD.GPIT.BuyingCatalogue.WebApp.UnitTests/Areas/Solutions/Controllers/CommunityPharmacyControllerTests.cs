using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class CommunityPharmacyControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CommunityPharmacyController)
                .Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "CommunityPharmacy");
            typeof(CommunityPharmacyController)
                .Should().BeDecoratedWith<RouteAttribute>(x => x.Template == "community-pharmacy");
        }

        [Theory]
        [MockAutoData]
        public static async Task ApplicationTypes_PublishedSolution_ReturnsCommunityPharmacyModel(
            Solution solution,
            CatalogueItemContentStatus contentStatus,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            catalogueItem.PublishedStatus = PublicationStatus.Published;
            solution.ApplicationTypeDetail = new ApplicationTypeDetail
            {
                ApplicationTypes = new() { ApplicationType.BrowserBased.EnumMemberName() },
                BrowsersSupported = new() { new() { BrowserName = "Chrome", MinimumBrowserVersion = "100" } },
            };
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);
            solutionsService.GetContentStatusForCatalogueItem(catalogueItem.Id).Returns(contentStatus);

            var result = (await controller.ApplicationTypes(catalogueItem.Id)).Should().BeOfType<ViewResult>().Subject;
            var model = result.Model.Should().BeOfType<CommunityPharmacyApplicationTypesModel>().Subject;

            result.ViewName.Should().BeNullOrEmpty();
            model.ApplicationTypesDetails.ApplicationTypes.Items.Should().ContainKey("Browser-based application");
            model.ApplicationTypesDetails.BrowserBasedApplication.Items["Supported browser types"].List
                .Should().Equal("Chrome (Version 100)");
            model.ApplicationTypesDetails.NativeDesktopApplication.Should().BeNull();
            model.ApplicationTypesDetails.NativeMobileApplication.Should().BeNull();
            model.SelectedSection.Should().Be("Application type");
            model.Sections.Single(s => s.Selected).Action.Should().Be(nameof(CommunityPharmacyController.ApplicationTypes));
            model.BreadcrumbItems.Last().Controller.Should().Be("CommunityPharmacy");
            model.PaginationFooter.Previous.Name.Should().Be("Integrations");
            model.PaginationFooter.Next.Name.Should().Be("Hosting type");
            model.PaginationFooter.FullWidth.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static async Task ApplicationTypes_MissingSolution_ReturnsBadRequest(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            solutionsService.GetSolutionWithBasicInformation(solutionId).Returns(default(CatalogueItem));

            var result = (await controller.ApplicationTypes(solutionId)).Should().BeOfType<BadRequestObjectResult>().Subject;

            result.Value.Should().Be($"No Catalogue Item found for Id: {solutionId}");
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task ApplicationTypes_SuspendedSolution_RedirectsToSummary(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            CommunityPharmacyController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Suspended;
            solutionsService.GetSolutionWithBasicInformation(catalogueItem.Id).Returns(catalogueItem);

            var result = (await controller.ApplicationTypes(catalogueItem.Id)).Should().BeOfType<RedirectToActionResult>().Subject;

            result.ActionName.Should().Be(nameof(CommunityPharmacyController.Summary));
            result.RouteValues["solutionId"].Should().Be(catalogueItem.Id);
            await solutionsService.DidNotReceive().GetContentStatusForCatalogueItem(Arg.Any<CatalogueItemId>());
        }

        [Theory]
        [MockAutoData]
        public static async Task GetSolutions_ReturnsExpectedModelAndView(
            string search,
            IList<CatalogueItem> catalogueItems,
            PageOptions returnedOptions,
            [Frozen] ISolutionsFilterService solutionsFilterService,
            CommunityPharmacyController controller)
        {
            const string page = "2";

            solutionsFilterService.GetAllSolutionsFiltered(
                    Arg.Is<SolutionsFilters>(filters => filters.Search == search && filters.IsCommunityPharmacy == true),
                    Arg.Any<PageOptions>())
                .Returns((catalogueItems, returnedOptions, null));

            var result = await controller.Solutions(page, null, search);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CommunityPharmacySolutionsModel>().Subject;

            model.ResultsModel.CatalogueItems.Should().BeEquivalentTo(catalogueItems);
            model.ResultsModel.PageOptions.Should().BeSameAs(returnedOptions);

            await solutionsFilterService.Received(1).GetAllSolutionsFiltered(
                Arg.Is<SolutionsFilters>(filters => filters.Search == search && filters.IsCommunityPharmacy == true),
                Arg.Any<PageOptions>());
        }

        [Theory]
        [MockAutoData]
        public static async Task GetFilterSearchSuggestions_ReturnsJsonResult(
            string search,
            Uri uri,
            List<SearchFilterModel> searchResults,
            [Frozen] ISolutionsFilterService solutionsFilterService,
            CommunityPharmacyController controller)
        {
            solutionsFilterService.GetSolutionsBySearchTerm(search, Arg.Any<int>(), true).Returns(searchResults);

            var context = new DefaultHttpContext();
            context.Request.Headers.Referer = uri.ToString();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context,
            };

            var currentPageUrl = new UriBuilder(uri.ToString());
            var expectedResults = searchResults.Select(r =>
                new HtmlEncodedSuggestionSearchResult(
                    r.Title,
                    r.Category,
                    currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).Uri.PathAndQuery));

            var result = await controller.FilterSearchSuggestions(search);

            await solutionsFilterService.Received(1).GetSolutionsBySearchTerm(search, Arg.Any<int>(), true);
            var actionResult = result.Should().BeOfType<JsonResult>().Subject;
            actionResult.Value.Should().BeEquivalentTo(expectedResults);
        }
    }
}
