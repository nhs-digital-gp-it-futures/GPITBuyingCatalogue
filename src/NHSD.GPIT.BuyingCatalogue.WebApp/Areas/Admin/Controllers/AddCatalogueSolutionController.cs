using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/add-solution")]
    public sealed class AddCatalogueSolutionController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public AddCatalogueSolutionController(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await solutionsService.GetAllSuppliers();

            return View(new AddSolutionModel().WithSuppliers(suppliers));
        }

        [HttpPost]
        public async Task<IActionResult> Index(AddSolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                var suppliers = await solutionsService.GetAllSuppliers();

                return View(model.WithSuppliers(suppliers));
            }

            var latestCatalogueItemId = await solutionsService.GetLatestCatalogueItemIdFor(model.SupplierId);

            await solutionsService.AddCatalogueSolution(new CatalogueItem
            {
                CatalogueItemId = latestCatalogueItemId.NextSolutionId(),
                CatalogueItemType = CatalogueItemType.Solution,
                Name = model.SolutionName,
                PublishedStatus = PublicationStatus.Draft,
                SupplierId = model.SupplierId,
            });

            return RedirectToAction(
                nameof(CatalogueSolutionsController.Index),
                typeof(CatalogueSolutionsController).ControllerName());
        }
    }
}
