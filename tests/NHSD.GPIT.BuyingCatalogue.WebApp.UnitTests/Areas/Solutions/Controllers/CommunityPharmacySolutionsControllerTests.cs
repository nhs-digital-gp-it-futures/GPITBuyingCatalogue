using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class CommunityPharmacySolutionsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CommunityPharmacySolutionsController)
                .Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Solutions");
            typeof(CommunityPharmacySolutionsController)
                .Should().BeDecoratedWith<RouteAttribute>(x => x.Template == "community-pharmacy/solutions");
        }

        [Theory]
        [MockAutoData]
        public static async Task GetIndex_ReturnsExpectedModelAndView(
            string search,
            IList<CatalogueItem> catalogueItems,
            PageOptions returnedOptions,
            [Frozen] ISolutionsFilterService solutionsFilterService,
            CommunityPharmacySolutionsController controller)
        {
            const string page = "2";

            solutionsFilterService.GetAllSolutionsFiltered(
                    Arg.Is<SolutionsFilters>(filters => filters.Search == search && filters.IsCommunityPharmacy == true),
                    Arg.Any<PageOptions>())
                .Returns((catalogueItems, returnedOptions, null));

            var result = await controller.Index(page, null, search);

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
            CommunityPharmacySolutionsController controller)
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
