using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public class CatalogueSolutionsController : Controller
    {
        private readonly ILogWrapper<CatalogueSolutionsController> logger;
        private readonly ISolutionsService solutionsService;

        public CatalogueSolutionsController(
            ILogWrapper<CatalogueSolutionsController> logger,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        public async Task<IActionResult> Index()
        {
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(Index)}");

            var solutions = await solutionsService.GetAllSolutions();

            var solutionModel = new CatalogueSolutionsModel()
            {
                CatalogueItems = solutions,
            };

            return View(solutionModel);
        }
    }
}
