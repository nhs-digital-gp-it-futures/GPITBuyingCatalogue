using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin")]
    public class HomeController : Controller
    {
        private readonly IOrganisationsService organisationsService;
        private readonly IMapper mapper;

        public HomeController(
            IOrganisationsService organisationsService,
            IMapper mapper)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [Route("buyer-organisations")]
        public async Task<IActionResult> BuyerOrganisations()
        {
            var organisations = await organisationsService.GetAllOrganisations();

            return View(mapper.Map<IList<Organisation>, IList<OrganisationModel>>(organisations));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
