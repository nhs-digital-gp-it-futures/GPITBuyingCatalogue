using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ImportModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
[Area("Admin")]
[Route("admin/import")]
public class ImportController : Controller
{
    private readonly IGpPracticeService gpPracticeService;
    private readonly IUsersService usersService;

    public ImportController(
        IGpPracticeService gpPracticeService,
        IUsersService usersService)
    {
        this.gpPracticeService = gpPracticeService ?? throw new ArgumentNullException(nameof(gpPracticeService));
        this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    [HttpGet("gp-practice-list")]
    public IActionResult ImportGpPracticeList()
    {
        return View(new ImportGpPracticeListModel
        {
            BackLink = Url.Action(
                nameof(OrganisationsController.Index),
                typeof(OrganisationsController).ControllerName()),
        });
    }

    [HttpPost("gp-practice-list")]
    public async Task<IActionResult> ImportGpPracticeList(ImportGpPracticeListModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await usersService.GetUser(User.UserId());

        await gpPracticeService.ImportGpPracticeData(new(model.CsvUrl), user.Email);

        return RedirectToAction(nameof(ImportGpPracticeListConfirmation));
    }

    [HttpGet("gp-practice-list/confirmation")]
    public IActionResult ImportGpPracticeListConfirmation()
    {
        return View(new NavBaseModel
        {
            BackLink = Url.Action(
                nameof(OrganisationsController.Index),
                typeof(OrganisationsController).ControllerName()),
        });
    }
}
