using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers
{
    [Authorize(Policy = "Buyer")]
    [Area("Solutions")]
    [Route("manage-filters")]
    public class ManageFiltersController : Controller
    {
        private readonly IOrganisationsService organisationsService;
        private readonly IManageFiltersService manageFiltersService;

        public ManageFiltersController(
            IOrganisationsService organisationsService,
            IManageFiltersService manageFiltersService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("save-filter")]
        public async Task<IActionResult> SaveFilter()
        {
            //ToDo replace with actual values from filtering
            var capabilities = new List<Capability>()
            {
                new Capability(){Id = 1 , Name = "Appointments Management – Citizen" },
                new Capability(){Id = 10 , Name = "GP Extracts Verification" },
            };
            var epics = new List<Epic>() { 
                new Epic() { Id = "S020X01E05", Name = "Supplier-defined epic 5", Capability =  capabilities[0]}, 
                new Epic() { Id = "S020X01E06", Name = "Supplier-defined epic 6", Capability = capabilities[0] },
                new Epic() { Id = "C10E1", Name = "View GPES payment extract reports", Capability = capabilities[1] },
            };
            var framework = new EntityFramework.Catalogue.Models.Framework() { Id = "DFOCVC001", ShortName = "DFOCVC" };
            var clientApplicationTypes = new List<ClientApplicationType>() { ClientApplicationType.BrowserBased };
            var hostingTypes = new List<HostingType>() { HostingType.Hybrid, HostingType.OnPremise };

            var organisationId = await GetUserOrganisationId();
            var model = new SaveFilterModel(capabilities, epics, framework, clientApplicationTypes, hostingTypes, organisationId) 
                { 
                    BackLink = "/", 
                    BackLinkText = "Go back",
                };
            return View(model);
        }

        [HttpPost("save-filter")]
        public async Task<IActionResult> SaveFilter(
            SaveFilterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await manageFiltersService.SaveFilter(
                model.Name, 
                model.Description, 
                model.OrganisationId, 
                model.CapabilityIds, 
                model.EpicIds, 
                model.FrameworkId, 
                model.ClientApplicationTypes, 
                model.HostingTypes);

            return RedirectToAction(
                nameof(Index),
                typeof(ManageFiltersController).ControllerName());
        }

        private async Task<int> GetUserOrganisationId()
        {
            var organisationInternalIdentifier = User.GetPrimaryOrganisationInternalIdentifier();
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(organisationInternalIdentifier);
            return organisation.Id;
        }
    }
}
