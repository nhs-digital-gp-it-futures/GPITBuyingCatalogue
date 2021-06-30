using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public sealed class CatalogueSolutionsController : Controller
    {
        private readonly ISolutionsService solutionsService;

        public CatalogueSolutionsController(
            ISolutionsService solutionsService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var solutions = await solutionsService.GetAllSolutions();

            var solutionModel = new CatalogueSolutionsModel(solutions);

            return View(solutionModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PublicationStatus publicationStatus)
        {
            var solutions = await solutionsService.GetAllSolutions(publicationStatus);

            var solutionModel = new CatalogueSolutionsModel(solutions);
            solutionModel.SetSelected(publicationStatus);

            return View(solutionModel);
        }
    }
}
