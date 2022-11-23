using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoreLinq;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
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
        public const int NumberOfOrders = 10;

        private readonly ICreateUserService createUserService;
        private readonly IOrderService orderService;
        private readonly IOrganisationsService organisationsService;
        private readonly IUsersService usersService;

        public UsersController(
            ICreateUserService createUserService,
            IOrderService orderService,
            IOrganisationsService organisationsService,
            IUsersService usersService)
        {
            this.createUserService = createUserService ?? throw new ArgumentNullException(nameof(createUserService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
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

            return Json(users.Take(15).Select(x => new SuggestionSearchResult
            {
                Title = x.FullName,
                Category = x.Email,
                Url = Url.Action(nameof(Details), new { userId = $"{x.Id}" }),
            }));
        }

        [HttpGet("add")]
        public async Task<IActionResult> Add()
        {
            var organisations = await organisationsService.GetAllOrganisations();

            return View(new AddModel
            {
                BackLink = Url.Action(nameof(Index)),
                Organisations = GetFormattedOrganisations(organisations),
            });
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(AddModel model)
        {
            if (!ModelState.IsValid)
            {
                var organisations = await organisationsService.GetAllOrganisations();

                model.Organisations = GetFormattedOrganisations(organisations);

                return View(model);
            }

            await createUserService.Create(
                int.Parse(model.SelectedOrganisationId),
                model.FirstName,
                model.LastName,
                model.Email,
                model.SelectedAccountType);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Details(int userId)
        {
            var user = await usersService.GetUser(userId);

            if (user == null)
            {
                return new NotFoundResult();
            }

            var roles = await usersService.GetRoles(user);
            var orders = await orderService.GetUserOrders(userId);

            return View(new DetailsModel
            {
                User = user,
                UserRole = roles.OrderBy(r => r).First(),
                Orders = orders.Take(NumberOfOrders).ToList(),
            });
        }

        [HttpGet("{userId}/account-status")]
        public async Task<IActionResult> AccountStatus(int userId)
        {
            var user = await usersService.GetUser(userId);

            if (user == null)
            {
                return new NotFoundResult();
            }

            var status = user.Disabled
                ? ServiceContracts.Enums.AccountStatus.Inactive
                : ServiceContracts.Enums.AccountStatus.Active;

            return View(new AccountStatusModel
            {
                BackLink = Url.Action(nameof(Details), new { userId }),
                Email = user.Email,
                SelectedAccountStatusId = $"{status}",
            });
        }

        [HttpPost("{userId}/account-status")]
        public async Task<IActionResult> AccountStatus(int userId, AccountStatusModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var disabled = model.SelectedAccountStatus == ServiceContracts.Enums.AccountStatus.Inactive;

            await usersService.EnableOrDisableUser(userId, disabled);

            return RedirectToAction(nameof(Details), new { userId });
        }

        [HttpGet("{userId}/account-type")]
        public async Task<IActionResult> AccountType(int userId)
        {
            var user = await usersService.GetUser(userId);

            if (user == null)
            {
                return new NotFoundResult();
            }

            var roles = await usersService.GetRoles(user);
            return View(new AccountTypeModel
            {
                BackLink = Url.Action(nameof(Details), new { userId }),
                Email = user.Email,
                SelectedAccountType = roles.OrderBy(r => r).First(),
            });
        }

        [HttpPost("{userId}/account-type")]
        public async Task<IActionResult> AccountType(int userId, AccountTypeModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await usersService.UpdateUserAccountType(userId, model.SelectedAccountType);

            return RedirectToAction(nameof(Details), new { userId });
        }

        [HttpGet("{userId}/organisation")]
        public async Task<IActionResult> Organisation(int userId)
        {
            var user = await usersService.GetUser(userId);

            if (user == null)
            {
                return new NotFoundResult();
            }

            var organisations = await organisationsService.GetAllOrganisations();

            return View(new OrganisationModel
            {
                BackLink = Url.Action(nameof(Details), new { userId }),
                UserId = userId,
                Email = user.Email,
                SelectedOrganisationId = $"{user.PrimaryOrganisationId}",
                Organisations = GetFormattedOrganisations(organisations),
            });
        }

        [HttpPost("{userId}/organisation")]
        public async Task<IActionResult> Organisation(int userId, OrganisationModel model)
        {
            if (!ModelState.IsValid)
            {
                var organisations = await organisationsService.GetAllOrganisations();

                model.Organisations = GetFormattedOrganisations(organisations);

                return View(model);
            }

            await usersService.UpdateUserOrganisation(userId, int.Parse(model.SelectedOrganisationId));

            return RedirectToAction(nameof(Details), new { userId });
        }

        [HttpGet("{userId}/personal-details")]
        public async Task<IActionResult> PersonalDetails(int userId)
        {
            var user = await usersService.GetUser(userId);

            if (user == null)
            {
                return new NotFoundResult();
            }

            return View(new PersonalDetailsModel
            {
                BackLink = Url.Action(nameof(Details), new { userId }),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            });
        }

        [HttpPost("{userId}/personal-details")]
        public async Task<IActionResult> PersonalDetails(int userId, PersonalDetailsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await usersService.UpdateUserDetails(
                userId,
                model.FirstName,
                model.LastName,
                model.Email);

            return RedirectToAction(nameof(Details), new { userId });
        }

        private static IEnumerable<SelectListItem> GetFormattedOrganisations(IEnumerable<Organisation> organisations)
        {
            return organisations.Select(x => new SelectListItem(x.Name, $"{x.Id}"));
        }

        private async Task<List<AspNetUser>> GetFilteredUsers(string searchTerm)
        {
            return string.IsNullOrWhiteSpace(searchTerm)
                ? await usersService.GetAllUsers()
                : await usersService.GetAllUsersBySearchTerm(searchTerm);
        }
    }
}
