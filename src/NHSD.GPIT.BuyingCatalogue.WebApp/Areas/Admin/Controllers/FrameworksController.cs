using System;
using System.Linq;
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
    public IActionResult Add() => View(new AddFrameworkModel { BackLink = Url.Action(nameof(Dashboard)) });

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddFrameworkModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await frameworkService.AddFramework(model.Name, model.FundingTypes.Where(x => x.Selected).Select(x => x.Value));

        return RedirectToAction(nameof(Dashboard));
    }

    [HttpGet("edit/{frameworkId}")]
    public async Task<IActionResult> Edit(string frameworkId)
    {
        var framework = await frameworkService.GetFramework(frameworkId);
        if (framework is null)
            return RedirectToAction(nameof(Dashboard));

        var view = View("Add", new AddFrameworkModel
        {
            BackLink = Url.Action(nameof(Dashboard)),
            FrameworkId = frameworkId,
            Name = framework.Name,
            //TODO: Populate selected funding types
            //FundingTypes = framework.FundingTypes.
            //FundingTypes.Select(x => x.Selected).Where(x => x.Selected)
            /*FundingTypes = framework.FundingTypes.Select(x => new SelectionModel
            {
                Id = $"{x.Id}",
                Selected = selected.Contains(x.Id),
            }).ToArray(),*/
        //FundingTypes.S.Where(x =>
        //FundingTypes = (System.Collections.Generic.List<Framework.Models.SelectOption<EntityFramework.Catalogue.Models.FundingType>>)framework.FundingTypes,
    });
        return view;
    }

    [HttpPost("edit/{frameworkId}")]
    public async Task<IActionResult> Edit(string frameworkId, AddFrameworkModel model)
    {
        if (!ModelState.IsValid)
            return View("add", model);
        //TODO: Validation won't let you keep same name

        await frameworkService.UpdateFramework(frameworkId, model.Name, model.FundingTypes.Where(x => x.Selected).Select(x => x.Value));

        return RedirectToAction(nameof(Dashboard));
    }

    [HttpGet("expire/{frameworkId}")]
    public async Task<IActionResult> Expire(string frameworkId)
    {
        var framework = await frameworkService.GetFramework(frameworkId);
        if (framework is null)
            return RedirectToAction(nameof(Dashboard));

        var model = new ExpireFrameworkModel
        {
            Name = framework.ShortName, BackLink = Url.Action(nameof(Dashboard), new { frameworkId }),
        };

        return View(model);
    }

    [HttpPost("expire/{frameworkId}")]
    public async Task<IActionResult> Expire(string frameworkId, ExpireFrameworkModel model)
    {
        _ = model;

        await frameworkService.MarkAsExpired(frameworkId);

        return RedirectToAction(nameof(Dashboard));
    }
}
