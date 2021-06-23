using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public class CatalogueSolutionsController : Controller
    {
        private readonly ILogWrapper<CatalogueSolutionsController> logger;
        private readonly ISolutionsService solutionsService;
        private readonly IMapper mapper;

        public CatalogueSolutionsController(
            ILogWrapper<CatalogueSolutionsController> logger,
            ISolutionsService solutionsService,
            IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IActionResult> Index()
        {
            var solutions = await solutionsService.GetAllSolutions();

            var solutionModel = mapper.Map<Ca>();

            return View(solutionModel);
        }
    }
}
