using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
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
    }
}
