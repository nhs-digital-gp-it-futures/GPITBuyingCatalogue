using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/catalogue-solutions")]
    public class CatalogueSolutionsController : Controller
    {
        private readonly ILogWrapper<HomeController> logger;

        public CatalogueSolutionsController(ILogWrapper<HomeController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(string id)
        {
            logger.LogInformation($"Taking user to {nameof(CatalogueSolutionsController)}.{nameof(Index)}");

            var solutionModel = new CatalogueSolutionsModel()
            {
                CatalogueItems = DummyData(),
            };

            return View(solutionModel);
        }


        private List<CatalogueItem> DummyData() 
        {
            List<CatalogueItem> items = new List<CatalogueItem>
            {
                new CatalogueItem()
                {
                    CatalogueItemId = "1000",
                    Name = "abc",
                    SupplierId = "1",
                    PublishedStatusId = 1,

                },
                new CatalogueItem()
                {
                    CatalogueItemId = "22222",
                    Name = "asdfasdfasdf",
                    SupplierId = "333",
                    PublishedStatusId = 3,

                },
            };

            return items;
        }
    }
}
