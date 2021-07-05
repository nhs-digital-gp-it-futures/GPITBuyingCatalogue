using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/add-solution")]
    public sealed class CatalogueSolutionController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public CatalogueSolutionController(ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await solutionsService.GetAllSuppliers();

            return View(new AddSolutionModel
            {
                Suppliers = suppliers?.ToDictionary(s => s.Id, s => s.Name),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(AddSolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                var suppliers = await solutionsService.GetAllSuppliers();
                var dictionary = suppliers?.ToDictionary(x => x.Id, x => x.Name);

                model.Suppliers = dictionary;
                return View(model);
            }

            var catalogueItemId = await solutionsService.GetLatestCatalogueItemIdFor(model.SupplierId);

            await solutionsService.AddCatalogueSolution(new CatalogueItem
            {
                CatalogueItemId = catalogueItemId.NextSolutionId(),
                Name = model.SolutionName,
                SupplierId = model.SupplierId,
            });

            return RedirectToAction(
                nameof(HomeController.Index),
                typeof(HomeController).ControllerName());
        }
    }
}
