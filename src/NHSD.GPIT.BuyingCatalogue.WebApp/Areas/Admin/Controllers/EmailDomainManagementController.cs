using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.EmailDomainManagement;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
[Area("Admin")]
[Route("admin/email-domains")]
public class EmailDomainManagementController : Controller
{
    private readonly IEmailDomainService emailDomainService;

    public EmailDomainManagementController(IEmailDomainService emailDomainService)
    {
        this.emailDomainService = emailDomainService ?? throw new ArgumentNullException(nameof(emailDomainService));
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var emailDomains = await emailDomainService.GetAllowedDomains();

        var model = new ViewEmailDomainsModel(emailDomains);

        return View(model);
    }

    [HttpGet("add")]
    public IActionResult AddEmailDomain()
    {
        var model = new AddEmailDomainModel { BackLink = Url.Action(nameof(Index)) };

        return View(model);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddEmailDomain(AddEmailDomainModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await emailDomainService.AddAllowedDomain(model.EmailDomain);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> DeleteEmailDomain(int id)
    {
        var emailDomain = await emailDomainService.GetAllowedDomain(id);
        if (emailDomain == null)
            return RedirectToAction(nameof(Index));

        var model = new DeleteEmailDomainModel(emailDomain) { BackLink = Url.Action(nameof(Index)) };

        return View(model);
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteEmailDomain(int id, DeleteEmailDomainModel model)
    {
        await emailDomainService.DeleteAllowedDomain(id);

        return RedirectToAction(nameof(Index));
    }
}
