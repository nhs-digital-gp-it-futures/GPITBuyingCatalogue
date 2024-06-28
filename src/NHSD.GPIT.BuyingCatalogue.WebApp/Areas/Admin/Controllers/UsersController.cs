using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/users")]
    public class UsersController : Controller
    {
        public const int PageSize = 10;

        private readonly ICreateUserService createUserService;
        private readonly IOrganisationsService organisationsService;
        private readonly IUsersService usersService;

        public UsersController(
            ICreateUserService createUserService,
            IOrganisationsService organisationsService,
            IUsersService usersService)
        {
            this.createUserService = createUserService ?? throw new ArgumentNullException(nameof(createUserService));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] string page = null,
            [FromQuery] string search = null)
        {
            var users = await GetFilteredUsers(search);

            var pageOptions = new PageOptions(page, PageSize)
            {
                TotalNumberOfItems = users.Count,
            };

            users = users.Skip(pageOptions.NumberToSkip).ToList();

            return View(new IndexModel
            {
                PageOptions = pageOptions,
                SearchTerm = search,
                Users = users.Take(PageSize),
            });
        }

        [HttpGet("search-results")]
        public async Task<IActionResult> SearchResults([FromQuery] string search)
        {
            var users = await GetFilteredUsers(search);

            return Json(users.Take(15).Select(x => new HtmlEncodedSuggestionSearchResult(
                x.FullName,
                x.Email,
                Url.Action(nameof(Edit), new { userId = $"{x.Id}" }))));
        }

        [HttpGet("add")]
        public async Task<IActionResult> Add()
        {
            var organisations = await organisationsService.GetAllOrganisations();

            return View("UserDetails", new Models.UserModels.UserDetailsModel
            {
                BackLink = Url.Action(nameof(Index)),
                Organisations = GetFormattedOrganisations(organisations),
            });
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(Models.UserModels.UserDetailsModel model)
        {
            if (!ModelState.IsValid)
            {
                var organisations = await organisationsService.GetAllOrganisations();

                model.Organisations = GetFormattedOrganisations(organisations);

                return View("UserDetails", model);
            }

            await createUserService.Create(
                model.SelectedOrganisationId!.Value,
                model.FirstName,
                model.LastName,
                model.Email,
                model.SelectedAccountType,
                !model.IsActive!.Value);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{userId}/edit")]
        public async Task<IActionResult> Edit(int userId)
        {
            var organisations = await organisationsService.GetAllOrganisations();
            var user = await usersService.GetUser(userId);

            if (user == null)
                return NotFound();

            var model = new Models.UserModels.UserDetailsModel(user)
            {
                BackLink = Url.Action(nameof(Index)),
                Organisations = GetFormattedOrganisations(organisations),
            };

            return View("UserDetails", model);
        }

        [HttpPost("{userId}/edit")]
        public async Task<IActionResult> Edit(int userId, Models.UserModels.UserDetailsModel model)
        {
            if (!ModelState.IsValid)
            {
                var organisations = await organisationsService.GetAllOrganisations();
                model.Organisations = GetFormattedOrganisations(organisations);
                return View("UserDetails", model);
            }

            var user = await usersService.GetUser(userId);

            if (user == null)
                return NotFound();

            await usersService.UpdateUser(
                userId,
                model.FirstName,
                model.LastName,
                model.Email,
                !model.IsActive!.Value,
                model.SelectedAccountType,
                model.SelectedOrganisationId!.Value);

            return RedirectToAction(nameof(Index));
        }

        private static IEnumerable<SelectOption<string>> GetFormattedOrganisations(IEnumerable<Organisation> organisations)
        {
            return organisations.Select(x => new SelectOption<string>(x.Name, $"{x.Id}"));
        }

        private async Task<List<AspNetUser>> GetFilteredUsers(string searchTerm)
        {
            return string.IsNullOrWhiteSpace(searchTerm)
                ? await usersService.GetAllUsers()
                : await usersService.GetAllUsersBySearchTerm(searchTerm);
        }
    }
}
