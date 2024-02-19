using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
[Area("Admin")]
[Route("admin/capabilities-mapping")]
public class CapabilitiesMappingController : Controller
{
    private const string InvalidCsvFormat = "File not formatted correctly";

    private readonly IGen2MappingService gen2MappingService;

    public CapabilitiesMappingController(
        IGen2MappingService gen2MappingService)
    {
        this.gen2MappingService = gen2MappingService
            ?? throw new ArgumentNullException(nameof(gen2MappingService));
    }

    [HttpGet("capabilities")]
    public IActionResult Capabilities()
    {
        return View(new CapabilitiesUploadModel());
    }

    [HttpPost("capabilities")]
    public async Task<IActionResult> Capabilities(CapabilitiesUploadModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var gen2Import = await gen2MappingService.GetCapabilitiesFromCsv(
            model.CapabilitiesCsv.FileName,
            model.CapabilitiesCsv.OpenReadStream());

        if (gen2Import == null || gen2Import.Imported?.Count == 0)
        {
            ModelState.AddModelError(nameof(model.CapabilitiesCsv), InvalidCsvFormat);

            return View(model);
        }

        var cacheId = await gen2MappingService.AddToCache(gen2Import);

        return RedirectToAction(
            gen2Import.Failed.Count > 0 ? nameof(FailedCapabilities) : nameof(Epics),
            new { id = cacheId });
    }

    [HttpGet("{id}/failed-capabilities")]
    public async Task<IActionResult> FailedCapabilities(Guid id)
    {
        var cachedImport = await gen2MappingService.GetCachedCapabilities(id);

        return View(new FailedCapabilitiesUploadModel(cachedImport.FileName, cachedImport.Failed));
    }

    [HttpPost("{id}/failed-capabilities")]
    public async Task<IActionResult> FailedCapabilities(Guid id, FailedCapabilitiesUploadModel model)
    {
        _ = model;

        var cachedImport = await gen2MappingService.GetCachedCapabilities(id);

        var failedImportCsv = await gen2MappingService.WriteCapabilitiesToCsv(cachedImport.Failed);

        return File(failedImportCsv, "text/csv");
    }

    [HttpGet("{id}/epics")]
    public IActionResult Epics(Guid id)
    {
        return View();
    }
}
