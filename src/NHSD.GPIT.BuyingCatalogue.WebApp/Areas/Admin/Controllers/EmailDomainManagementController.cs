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
}
