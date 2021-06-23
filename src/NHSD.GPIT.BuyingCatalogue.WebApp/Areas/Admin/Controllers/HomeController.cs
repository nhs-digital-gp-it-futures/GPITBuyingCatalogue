using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
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

        public HomeController(
            ILogWrapper<HomeController> logger,
            IOrganisationsService organisationsService,
            IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [Route("buyer-organisations")]
        public async Task<IActionResult> BuyerOrganisations()
        {
            logger.LogInformation($"Taking user to Admin {nameof(HomeController)}.{nameof(Index)}");

            var organisations = await organisationsService.GetAllOrganisations();
            var organisationModel = mapper.Map<IList<Organisation>, IList<OrganisationModel>>(organisations);

            return View(new ListOrganisationsModel(organisationModel));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
