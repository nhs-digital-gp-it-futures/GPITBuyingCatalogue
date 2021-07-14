using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public sealed class CatalogueSolutionsController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IUsersService usersService;

        public CatalogueSolutionsController(
            ISolutionsService solutionsService,
            IUsersService usersService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var solutions = await solutionsService.GetAllSolutions();

            var solutionModel = new CatalogueSolutionsModel(solutions);

            return View(solutionModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CatalogueSolutionsModel model)
        {
            model.SetSolutions(
                 await solutionsService.GetAllSolutions(string.IsNullOrWhiteSpace(model.SelectedPublicationStatus)
                     ? null
                     : Enum.Parse<PublicationStatus>(model.SelectedPublicationStatus, true)));

            return View(model);
        }

        [HttpGet("manage/{solutionId}")]
        public async Task<IActionResult> ManageCatalogueSolution(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolution(solutionId);

            var lastUpdatedBy = await usersService.GetUser(solution.Solution.LastUpdatedBy.ToString());

            var model = new ManageCatalogueSolutionModel
            {
                Solution = solution,
            };

            if (lastUpdatedBy is not null)
                model.LastUpdatedByName = $"{lastUpdatedBy.FirstName} {lastUpdatedBy.LastName}";

            return View(model);
        }
    }
}
