﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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
        private readonly IOrganisationsService organisationsService;
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public HomeController(
            IOrganisationsService organisationsService,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService =
                solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [Route("buyer-organisations")]
        public async Task<IActionResult> BuyerOrganisations()
        {
            var organisations = await organisationsService.GetAllOrganisations();

            return View(mapper.Map<IList<Organisation>, IList<OrganisationModel>>(organisations));
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

        public IActionResult Index()
        {
            return View();
        }
    }
}
