﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers
{
    [Area("Marketing")]
    [Route("marketing/supplier/solution/{id}")]
    public class SolutionController : Controller
    {
        private readonly ILogWrapper<SolutionController> logger;
        private readonly IMapper mapper;
        private readonly ISolutionsService solutionsService;

        public SolutionController(
            ILogWrapper<SolutionController> logger,
            IMapper mapper,
            ISolutionsService solutionsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            var solution = await solutionsService.GetSolution(id);

            if (solution == null)
                return BadRequest($"No Catalogue Item found for Id: {id}");

            var model = mapper.Map<CatalogueItem, SolutionStatusModel>(solution);

            return View(model);
        }

        [HttpGet("preview")]
        public IActionResult Preview(string id)
        {
            return RedirectToAction(
                nameof(SolutionDetailsController.PreviewSolutionDetail),
                typeof(SolutionDetailsController).ControllerName(),
                new { id });
        }
    }
}
