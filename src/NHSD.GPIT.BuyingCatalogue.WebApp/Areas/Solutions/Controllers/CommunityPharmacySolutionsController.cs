using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;

[Area("Solutions")]
[Route("community-pharmacy/solutions")]
public class CommunityPharmacySolutionsController(ISolutionsFilterService solutionsFilterService) : Controller
{
    private readonly ISolutionsFilterService solutionsFilterService = solutionsFilterService ?? throw new ArgumentNullException(nameof(solutionsFilterService));

    [HttpGet]
    public async Task<IActionResult> Index(
        [FromQuery] string page,
        [FromQuery] string sortBy,
        [FromQuery] string search)
    {
        var selectedSort = string.IsNullOrWhiteSpace(sortBy)
            ? PageOptions.SortOptions.LastPublished
            : Enum.Parse<PageOptions.SortOptions>(sortBy);

        var inputOptions = new PageOptions(page, selectedSort.ToString())
        {
            PageSize = 10,
        };

        var filters = new SolutionsFilters() { IsCommunityPharmacy = true, Search = search, };

        (IList<CatalogueItem> catalogueItems, PageOptions options, _) =
            await solutionsFilterService.GetAllSolutionsFiltered(
                filters,
                inputOptions);

        var model = new CommunityPharmacySolutionsModel
        {
            ResultsModel = new SolutionsResultsModel { CatalogueItems = catalogueItems, PageOptions = options },
        };

        return View(model);
    }
}
