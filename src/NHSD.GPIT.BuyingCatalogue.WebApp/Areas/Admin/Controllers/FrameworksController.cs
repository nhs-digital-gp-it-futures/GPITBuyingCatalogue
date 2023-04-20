using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
[Area("Admin")]
[Route("admin/frameworks")]
public class FrameworksController : Controller
{
    private readonly IFrameworkService frameworkService;

    public FrameworksController(
        IFrameworkService frameworkService)
    {
        this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var frameworks = await frameworkService.GetFrameworks();

        var model = new FrameworksDashboardModel(frameworks);

        return View(model);
    }

    [HttpGet("add")]
    public IActionResult Add() => View(new AddEditFrameworkModel { BackLink = Url.Action(nameof(Dashboard)) });

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddEditFrameworkModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await frameworkService.AddFramework(model.Name, model.IsLocalFundingOnly);

        return RedirectToAction(nameof(Dashboard));
    }

    [HttpGet("edit/{frameworkId}")]
    public async Task<IActionResult> Edit(string frameworkId)
    {
        var framework = await frameworkService.GetFramework(frameworkId);

        if (framework is null)
            return RedirectToAction(nameof(Dashboard));

        var model = new AddEditFrameworkModel
        {
            FrameworkId = frameworkId, Name = framework.ShortName, IsLocalFundingOnly = framework.LocalFundingOnly,
        };

        return View(model);
    }

    [HttpPost("edit/{frameworkId}")]
    public async Task<IActionResult> Edit(string frameworkId, AddEditFrameworkModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await frameworkService.EditFramework(frameworkId, model.Name, model.IsLocalFundingOnly);

        return RedirectToAction(nameof(Dashboard));
    }

    [HttpGet("delete/{frameworkId}")]
    public async Task<IActionResult> Delete(string frameworkId)
    {
        var framework = await frameworkService.GetFramework(frameworkId);
        if (framework is null)
            return RedirectToAction(nameof(Dashboard));

        var model = new DeleteFrameworkModel
        {
            Name = framework.ShortName, BackLink = Url.Action(nameof(Edit), new { frameworkId }),
        };

        return View(model);
    }

    [HttpPost("delete/{frameworkId}")]
    public async Task<IActionResult> Delete(string frameworkId, DeleteFrameworkModel model)
    {
        _ = model;

        await frameworkService.MarkAsExpired(frameworkId);

        return RedirectToAction(nameof(Dashboard));
    }
}
