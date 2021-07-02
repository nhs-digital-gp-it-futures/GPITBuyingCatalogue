using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

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

            return View(new AddSolutionModel().WithSelectListItems(suppliers));
        }

        [HttpPost]
        public async Task<IActionResult> Index(AddSolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                var suppliers = await solutionsService.GetAllSuppliers();

                return View(model.WithSelectListItems(suppliers));
            }

            await solutionsService.AddCatalogueSolution(new CreateSolutionModel
            {
                FrameworkModel = model.FrameworkModel,
                Name = model.SolutionName,
                SupplierId = model.SupplierId,
                UserId = User.UserId(),
            });

            return RedirectToAction(
                nameof(CatalogueSolutionsController.Index),
                typeof(CatalogueSolutionsController).ControllerName());
        }
    }
}
