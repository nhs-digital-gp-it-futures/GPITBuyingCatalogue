using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/add-solution")]
    public sealed class AddCatalogueSolutionController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly ISuppliersService suppliersService;

        public AddCatalogueSolutionController(ISolutionsService solutionsService, ISuppliersService suppliersService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.suppliersService = suppliersService ?? throw new ArgumentNullException(nameof(suppliersService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await suppliersService.GetAllActiveSuppliers();

            var model = new AddSolutionModel().WithSelectListItems(suppliers);

            model.Frameworks = (await solutionsService.GetAllFrameworks())
                .Select(f => new FrameworkModel { Name = $"{f.ShortName} Framework", FrameworkId = f.Id }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(AddSolutionModel model)
        {
            if (await solutionsService.GetSolutionByName(model.SolutionName) is not null)
                ModelState.AddModelError(nameof(AddSolutionModel.SolutionName), "A solution with this name already exists");

            if (!ModelState.IsValid)
            {
                var suppliers = await suppliersService.GetAllSuppliers();

                return View(model.WithSelectListItems(suppliers));
            }

            var catalogueItemId = await solutionsService.AddCatalogueSolution(new CreateSolutionModel
            {
                Frameworks = model.Frameworks,
                Name = model.SolutionName,

                // Model validation ensures supplier ID has a value
                SupplierId = model.SupplierId!.Value,
                UserId = User.UserId(),
            });

            return RedirectToAction(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionId = catalogueItemId });
        }
    }
}
