using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin")]
    public sealed class HomeController : Controller
    {
        private readonly ILogWrapper<HomeController> logger;
        private readonly IOrganisationsService organisationsService;
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public HomeController(
            ILogWrapper<HomeController> logger,
            IOrganisationsService organisationsService,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService =
                solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [Route("buyer-organisations")]
        public async Task<IActionResult> BuyerOrganisations()
        {
            logger.LogInformation($"Taking user to Admin {nameof(HomeController)}.{nameof(Index)}");

            var organisations = await organisationsService.GetAllOrganisations();
            var organisationModel = mapper.Map<IList<Organisation>, IList<OrganisationModel>>(organisations);

            return View(new ListOrganisationsModel(organisationModel));
        }

        [HttpGet("catalogue-solutions/add-solution")]
        public async Task<IActionResult> AddSolution()
        {
            var suppliers = await solutionsService.GetAllSuppliers();

            return View(new AddSolutionModel
            {
                Suppliers = suppliers?.ToDictionary(s => s.Id, s => s.Name),
            });
        }

        [Route("manage-suppliers")]
        public async Task<IActionResult> ManageSuppliers()
        {
            var suppliers = await solutionsService.GetAllSuppliers();
            return View(suppliers);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
