using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
[Area("Admin")]
[Route("admin/capabilities-mapping/{id}")]
public class Gen2MappingController : Controller
{
    private const string InvalidCsvFormat = "File not formatted correctly";

    private readonly IGen2UploadService gen2UploadService;

    public Gen2MappingController(
        IGen2UploadService gen2UploadService)
    {
        this.gen2UploadService = gen2UploadService
            ?? throw new ArgumentNullException(nameof(gen2UploadService));
    }

    [HttpGet("capabilities")]
    public IActionResult Capabilities(Guid id)
    {
        _ = id;

        return View(new Gen2UploadModel());
    }

    [HttpPost("capabilities")]
    public async Task<IActionResult> Capabilities(Guid id, Gen2UploadModel model) =>
        await HandleFileUpload(
            id,
            model,
            gen2UploadService.GetCapabilitiesFromCsv,
            gen2UploadService.AddToCache,
            nameof(Epics),
            nameof(FailedCapabilities));

    [HttpGet("failed-capabilities")]
    public async Task<IActionResult> FailedCapabilities(Guid id)
    {
        var cachedImport = await gen2UploadService.GetCachedCapabilities(id);

        return View(new FailedGen2UploadModel<Gen2CapabilitiesCsvModel>(cachedImport.Failed));
    }

    [HttpPost("failed-capabilities")]
    public async Task<IActionResult> FailedCapabilities(Guid id, FailedGen2UploadModel<Gen2CapabilitiesCsvModel> model)
    {
        _ = model;

        return await HandleFailedUpload(
            id,
            gen2UploadService.GetCachedCapabilities,
            gen2UploadService.WriteToCsv);
    }

    [HttpGet("epics")]
    public IActionResult Epics(Guid id)
    {
        _ = id;

        return View(new Gen2UploadModel());
    }

    [HttpPost("epics")]
    public async Task<IActionResult> Epics(Guid id, Gen2UploadModel model) =>
        await HandleFileUpload(
            id,
            model,
            gen2UploadService.GetEpicsFromCsv,
            gen2UploadService.AddToCache,
            nameof(Epics),
            nameof(FailedEpics));

    [HttpGet("failed-epics")]
    public async Task<IActionResult> FailedEpics(Guid id)
    {
        var cachedImport = await gen2UploadService.GetCachedEpics(id);

        return View(new FailedGen2UploadModel<Gen2EpicsCsvModel>(cachedImport.Failed));
    }

    [HttpPost("failed-epics")]
    public async Task<IActionResult> FailedEpics(Guid id, FailedGen2UploadModel<Gen2EpicsCsvModel> model)
    {
        _ = model;

        return await HandleFailedUpload(
            id,
            gen2UploadService.GetCachedEpics,
            gen2UploadService.WriteToCsv);
    }

    private async Task<IActionResult> HandleFileUpload<T>(
        Guid id,
        Gen2UploadModel model,
        Func<Stream, Task<Gen2CsvImportModel<T>>> readFunc,
        Func<Guid, Gen2CsvImportModel<T>, Task> cacheFunc,
        string successAction,
        string failedAction)
        where T : Gen2CsvBase
    {
        if (!ModelState.IsValid)
            return View(model);

        var gen2Import = await readFunc(
            model.File.OpenReadStream());

        if (gen2Import == null || gen2Import.Imported?.Count == 0)
        {
            ModelState.AddModelError(nameof(model.File), InvalidCsvFormat);

            return View(model);
        }

        await cacheFunc(id, gen2Import);

        return RedirectToAction(
            gen2Import.Failed.Count > 0 ? failedAction : successAction,
            new { id });
    }

    private async Task<IActionResult> HandleFailedUpload<T>(
        Guid id,
        Func<Guid, Task<Gen2CsvImportModel<T>>> cacheFunc,
        Func<IEnumerable<T>, Task<Stream>> csvFunc)
        where T : Gen2CsvBase
    {
        var cachedImport = await cacheFunc(id);

        var failedImportCsv = await csvFunc(cachedImport.Failed);

        return File(failedImportCsv, "text/csv");
    }
}
