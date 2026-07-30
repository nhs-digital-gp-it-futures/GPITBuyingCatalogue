using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AccountRequestsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "ManageAccountCreationRequests")]
[Area("Admin")]
[Route("admin/users/account-requests")]
public class AccountRequestsController(IAccountRequestsService accountRequestsService) : Controller
{
    private readonly IAccountRequestsService accountRequestsService = accountRequestsService ?? throw new ArgumentNullException(nameof(accountRequestsService));

    [HttpGet]
    public async Task<IActionResult> Index(
        [FromQuery] string page = "",
        [FromQuery] AccountRequestStatus status = AccountRequestStatus.Pending)
    {
        var pageOptions = new PageOptions(page, pageSize: 10);
        var accountRequests = await accountRequestsService.GetAccountRequests(status, pageOptions);

        var model = new AccountRequestsDashboardModel(status, accountRequests);

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(AccountRequestsDashboardModel model) => RedirectToAction(nameof(Index), new { status = model.SelectedStatus });

    [HttpGet("{requestId}")]
    public async Task<IActionResult> ViewAccountRequest(Guid requestId)
    {
        var accountRequest = await accountRequestsService.GetAccountRequest(requestId);

        var model = new AccountRequestModel(accountRequest) { BackLink = Url.Action(nameof(Index)), };

        return View(model);
    }

    [HttpPost("{requestId}")]
    public async Task<IActionResult> ViewAccountRequest(Guid requestId, AccountRequestModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await accountRequestsService.ProcessAccountRequest(requestId, User.UserId(), model.SelectedStatus.GetValueOrDefault(), model.DecisionJustification);

        return RedirectToAction(nameof(ViewAccountRequest), new { requestId });
    }
}
