using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public class CatalogueSolutionsController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IMapper mapper;

        public CatalogueSolutionsController(
            ISolutionsService solutionsService,
            IMapper mapper)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var solutions = await solutionsService.GetAllSolutions();

            var solutionModel = mapper.Map<IList<CatalogueItem>, CatalogueSolutionsModel>(solutions);

            return View(solutionModel);
        }
    }
}
