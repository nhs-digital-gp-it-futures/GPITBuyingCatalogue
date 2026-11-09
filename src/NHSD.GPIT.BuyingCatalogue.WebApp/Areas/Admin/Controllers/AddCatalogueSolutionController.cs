using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "ManageCatalogueSolutions")]
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

        [HttpGet("{solutionType}")]
        public async Task<IActionResult> Index(SolutionType solutionType)
        {
            var suppliers = await suppliersService.GetAllSuppliers();

            var model = new SolutionModel()
            {
                SolutionType = solutionType,
                BackLink = Url.Action(nameof(CatalogueSolutionsController.Index), typeof(CatalogueSolutionsController).ControllerName(), new { solutionType }),
            }.WithSelectListItems(suppliers);

            model.Frameworks = await GetFrameworks();

            return View("Details", model);
        }

        [HttpPost("{solutionType}")]
        public async Task<IActionResult> Index(SolutionType solutionType, SolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                var suppliers = await suppliersService.GetAllSuppliers();

                model.Frameworks = await GetFrameworks();

                return View("Details", model.WithSelectListItems(suppliers));
            }

            var catalogueItemId = await solutionsService.AddCatalogueSolution(new CreateSolutionModel
            {
                Frameworks = model.Frameworks,
                Name = model.SolutionName,
                SupplierId = model.SupplierId!.Value,
                IsPilotSolution = model.IsPilotSolution,
                UserId = User.UserId(),
            });

            return RedirectToAction(
                nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { solutionType, solutionId = catalogueItemId });
        }

        private async Task<IList<FrameworkModel>> GetFrameworks() => (await solutionsService.GetAllFrameworks())
                .Select(f => new FrameworkModel { Name = $"{f.ShortName} Framework", FrameworkId = f.Id }).ToList();
    }
}
