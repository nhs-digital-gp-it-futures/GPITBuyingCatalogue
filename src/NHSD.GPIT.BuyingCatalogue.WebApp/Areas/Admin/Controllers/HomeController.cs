using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin")]
    public sealed class HomeController : Controller
    {
        private readonly IOrganisationsService organisationsService;

        public HomeController(
            IOrganisationsService organisationsService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        }

        [HttpGet("buyer-organisations")]
        public async Task<IActionResult> BuyerOrganisations()
        {
            var organisations = await organisationsService.GetAllOrganisations();
            var organisationModel = organisations.Select(
                o => new OrganisationModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    OdsCode = o.InternalIdentifier,
                }).ToList();

            return View(new ListOrganisationsModel(organisationModel));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
