using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    [Route("ComponentTest")]
    [Area("ComponentTest")]
    public class ComponentTestController : Controller
    {
        [HttpGet]
        public IActionResult ComponentTest()
        {
            return View("ComponentTest", new Views.ComponentTestModel());
        }

        [HttpPost]
        public IActionResult ComponentTest(Views.ComponentTestModel model)
        {
            (var date, var error) = model.ToDateTime();

            if (error != null)
                ModelState.AddModelError("Day", error);

            return View("ComponentTest", model);
        }
    }
}
