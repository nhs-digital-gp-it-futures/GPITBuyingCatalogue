using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers
{
    [Authorize(Policy = "AccountManager")]
    [Area("AccountManagement")]
    [Route("accountmanagement")]
    public sealed class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // ToDo Placeholder. Currently Account Management does not have its own Home page so redirects to the buyer home page
            return RedirectToAction(
                nameof(WebApp.Controllers.HomeController.Index),
                typeof(WebApp.Controllers.HomeController).ControllerName(),
                new { area = string.Empty });
        }
    }
}
