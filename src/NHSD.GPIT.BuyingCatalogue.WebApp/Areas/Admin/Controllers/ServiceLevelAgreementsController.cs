using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Area("Admin")]
    [Route("admin/catalogue-solutions/manage/{solutionId}/service-level-agreements")]
    public class ServiceLevelAgreementsController : Controller
    {
        private readonly IServiceLevelAgreementsService serviceLevelAgreementsService;
        private readonly ISolutionsService solutionsService;

        public ServiceLevelAgreementsController(
            IServiceLevelAgreementsService serviceLevelAgreementsService,
            ISolutionsService solutionsService)
        {
            this.serviceLevelAgreementsService = serviceLevelAgreementsService ?? throw new ArgumentNullException(nameof(serviceLevelAgreementsService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var sla = await serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(solutionId);

            if (sla is null)
            {
                return RedirectToAction(nameof(AddServiceLevelAgreement), new { solutionId });
            }

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("add-sla")]
        public async Task<IActionResult> AddServiceLevelAgreement(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new AddSlaTypeModel(solution)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
                Title = "Catalogue Solution type",
            };

            return View("AddEditSlaType", model);
        }

        [HttpPost("add-sla")]
        public async Task<IActionResult> AddServiceLevelAgreement(CatalogueItemId solutionId, AddSlaTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddEditSlaType", model);
            }

            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var addSlaModel = new AddSlaModel
            {
                Solution = solution,
                SlaLevel = model.SlaLevel.Value,
            };

            await serviceLevelAgreementsService.AddServiceLevelAgreement(addSlaModel);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("edit-service-level-agreement")]
        public async Task<IActionResult> EditServiceLevelAgreement(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceLevelAgreements = await serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(solutionId);

            var model = new EditServiceLevelAgreementModel(solution, serviceLevelAgreements)
            {
                BackLink = Url.Action(
                    nameof(CatalogueSolutionsController.ManageCatalogueSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { solutionId }),
            };

            return View(model);
        }

        [HttpGet("edit-service-level-agreement/edit-sla-type")]
        public async Task<IActionResult> EditServiceLevelAgreementType(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceLevelAgreements = await serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(solutionId);

            var model = new AddSlaTypeModel(catalogueItem)
            {
                SlaLevel = serviceLevelAgreements.SlaType,
                BackLink = Url.Action(nameof(EditServiceLevelAgreement), new { solutionId }),
                Title = "Catalogue Solution type",
            };

            return View("AddEditSlaType", model);
        }

        [HttpPost("edit-service-level-agreement/edit-sla-type")]
        public async Task<IActionResult> EditServiceLevelAgreementType(CatalogueItemId solutionId, AddSlaTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddEditSlaType", model);
            }

            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            if (model.SlaLevel!.Value != solution.Solution.ServiceLevelAgreement.SlaType)
                return RedirectToAction(nameof(EditServiceLevelAgreementTypeConfirmation), new { solutionId });

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("edit-service-level-agreement/edit-sla-type/confirm-change")]
        public async Task<IActionResult> EditServiceLevelAgreementTypeConfirmation(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new EditSlaTypeConfirmationModel(catalogueItem)
            {
                BackLink = Url.Action(nameof(EditServiceLevelAgreementType), new { solutionId }),
            };

            return View(model);
        }

        [HttpPost("edit-service-level-agreement/edit-sla-type/confirm-change")]
        public async Task<IActionResult> EditServiceLevelAgreementTypeConfirmation(CatalogueItemId solutionId, EditSlaTypeConfirmationModel model)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var newSlaType = catalogueItem.Solution.ServiceLevelAgreement.SlaType == SlaType.Type1 ? SlaType.Type2 : SlaType.Type1;

            await serviceLevelAgreementsService.UpdateServiceLevelTypeAsync(catalogueItem, newSlaType);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("edit-service-level-agreement/add-service-availability-times")]
        public async Task<IActionResult> AddServiceAvailabilityTimes(CatalogueItemId solutionId)
        {
            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new EditServiceAvailabilityTimesModel(solution)
            {
                BackLink = Url.Action(nameof(EditServiceLevelAgreement), new { solutionId }),
            };

            return View("AddEditServiceAvailabilityTimes", model);
        }

        [HttpPost("edit-service-level-agreement/add-service-availability-times")]
        public async Task<IActionResult> AddServiceAvailabilityTimes(CatalogueItemId solutionId, EditServiceAvailabilityTimesModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEditServiceAvailabilityTimes", model);

            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceAvailabilityTimesModel = new ServiceAvailabilityTimesModel
            {
                SupportType = model.SupportType,
                From = model.From!.Value,
                Until = model.Until!.Value,
                ApplicableDays = model.ApplicableDays.Where(x => x.Selected).Select(x => x.Value).ToList(),
                IncludesBankHolidays = model.IncludesBankHolidays.GetValueOrDefault(),
                AdditionalInformation = model.AdditionalInformation,
                UserId = User.UserId(),
            };

            await serviceLevelAgreementsService.SaveServiceAvailabilityTimes(solution, serviceAvailabilityTimesModel);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("edit-service-level-agreement/edit-service-availability-times/{serviceAvailabilityTimesId}")]
        public async Task<IActionResult> EditServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId)
        {
            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceAvailabilityTimes = await serviceLevelAgreementsService.GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);
            if (serviceAvailabilityTimes is null)
                return BadRequest($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solutionId}");

            var model = new EditServiceAvailabilityTimesModel(solution, serviceAvailabilityTimes)
            {
                BackLink = Url.Action(nameof(EditServiceLevelAgreement), new { solutionId }),
                CanDelete = solution.PublishedStatus != PublicationStatus.Published || ((await serviceLevelAgreementsService.GetCountOfServiceAvailabilityTimes(solutionId, serviceAvailabilityTimes.Id)) > 0),
            };

            return View("AddEditServiceAvailabilityTimes", model);
        }

        [HttpPost("edit-service-level-agreement/edit-service-availability-times/{serviceAvailabilityTimesId}")]
        public async Task<IActionResult> EditServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId, EditServiceAvailabilityTimesModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEditServiceAvailabilityTimes", model);

            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceAvailabilityTimes = await serviceLevelAgreementsService.GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);
            if (serviceAvailabilityTimes is null)
                return BadRequest($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solutionId}");

            var serviceAvailabilityTimesModel = new ServiceAvailabilityTimesModel
            {
                SupportType = model.SupportType,
                From = model.From!.Value,
                Until = model.Until!.Value,
                ApplicableDays = model.ApplicableDays.Where(x => x.Selected).Select(x => x.Value).ToList(),
                IncludesBankHolidays = model.IncludesBankHolidays.GetValueOrDefault(),
                AdditionalInformation = model.AdditionalInformation,
                UserId = User.UserId(),
            };

            await serviceLevelAgreementsService.UpdateServiceAvailabilityTimes(solution, serviceAvailabilityTimesId, serviceAvailabilityTimesModel);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("edit-service-level-agreement/delete-service-availability-times/{serviceAvailabilityTimesId}")]
        public async Task<IActionResult> DeleteServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId)
        {
            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceAvailabilityTimes = await serviceLevelAgreementsService.GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);
            if (serviceAvailabilityTimes is null)
                return BadRequest($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solutionId}");

            var model = new DeleteServiceAvailabilityTimesModel(solution, serviceAvailabilityTimes)
            {
                BackLink = Url.Action(nameof(EditServiceAvailabilityTimes), new { solutionId, serviceAvailabilityTimesId }),
            };

            return View(model);
        }

        [HttpPost("edit-service-level-agreement/delete-service-availability-times/{serviceAvailabilityTimesId}")]
        public async Task<IActionResult> DeleteServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId, DeleteServiceAvailabilityTimesModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var solution = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (solution is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceAvailabilityTimes = await serviceLevelAgreementsService.GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);
            if (serviceAvailabilityTimes is null)
                return BadRequest($"No Service Availability Times with Id {serviceAvailabilityTimesId} found for Solution: {solutionId}");

            await serviceLevelAgreementsService.DeleteServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("add-contact")]
        public async Task<IActionResult> AddContact(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new Models.ServiceLevelAgreements.EditSLAContactModel(catalogueItem)
            {
                BackLink = Url.Action(nameof(EditServiceLevelAgreement), new { solutionId }),
            };

            return View("EditSLAContact", model);
        }

        [HttpPost("add-contact")]
        public async Task<IActionResult> AddContact(CatalogueItemId solutionId, Models.ServiceLevelAgreements.EditSLAContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditSLAContact", model);
            }

            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var addSLAContactModel = new ServiceContracts.Models.ServiceLevelAgreements.EditSLAContactModel
            {
                Channel = model.Channel,
                ContactInformation = model.ContactInformation,
                ApplicableDays = model.ApplicableDays,
                TimeFrom = model.From.Value,
                TimeUntil = model.Until.Value,
                UserId = User.UserId(),
            };

            await serviceLevelAgreementsService.AddSLAContact(catalogueItem, addSLAContactModel);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("edit-contact/{contactId}")]
        public async Task<IActionResult> EditContact(CatalogueItemId solutionId, int contactId)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceLevelAgreements = await serviceLevelAgreementsService.GetServiceLevelAgreementForSolution(solutionId);

            var contact = serviceLevelAgreements.Contacts.FirstOrDefault(slac => slac.Id == contactId);

            if (contact is null)
                return BadRequest($"No Contact found for Id: {contactId}");

            var model = new Models.ServiceLevelAgreements.EditSLAContactModel(catalogueItem, contact, serviceLevelAgreements)
            {
                BackLink = Url.Action(nameof(EditServiceLevelAgreement), new { solutionId }),
            };

            return View("EditSLAContact", model);
        }

        [HttpPost("edit-contact/{contactId}")]
        public async Task<IActionResult> EditContact(CatalogueItemId solutionId, int contactId, Models.ServiceLevelAgreements.EditSLAContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditSLAContact", model);
            }

            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            if (!catalogueItem.Solution.ServiceLevelAgreement.Contacts.Any(slac => slac.Id == contactId))
                return BadRequest($"No Contact found for Id: {contactId}");

            var addSLAContactModel = new ServiceContracts.Models.ServiceLevelAgreements.EditSLAContactModel
            {
                Id = contactId,
                Channel = model.Channel,
                ContactInformation = model.ContactInformation,
                ApplicableDays = model.ApplicableDays,
                TimeFrom = model.From.Value,
                TimeUntil = model.Until.Value,
                UserId = User.UserId(),
            };

            await serviceLevelAgreementsService.EditSlaContact(addSLAContactModel);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("delete-contact/{contactId}")]
        public async Task<IActionResult> DeleteContact(CatalogueItemId solutionId, int contactId)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            if (!catalogueItem.Solution.ServiceLevelAgreement.Contacts.Any(slac => slac.Id == contactId))
                return BadRequest($"No Contact found for Id: {contactId}");

            var contact = catalogueItem.Solution.ServiceLevelAgreement.Contacts.FirstOrDefault(slac => slac.Id == contactId);

            var model = new Models.ServiceLevelAgreements.EditSLAContactModel(catalogueItem, contact)
            {
                BackLink = Url.Action(nameof(EditContact), new { solutionId, contactId }),
            };

            return View("DeleteSLAContact", model);
        }

        [HttpPost("delete-contact/{contactId}")]
        public async Task<IActionResult> DeleteContact(CatalogueItemId solutionId, int contactId, Models.ServiceLevelAgreements.EditSLAContactModel model)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            if (!catalogueItem.Solution.ServiceLevelAgreement.Contacts.Any(slac => slac.Id == contactId))
                return BadRequest($"No Contact found for Id: {contactId}");

            await serviceLevelAgreementsService.DeleteSlaContact(contactId);

            return RedirectToAction(nameof(EditServiceLevelAgreement), new { solutionId });
        }

        [HttpGet("add-service-level")]
        public async Task<IActionResult> AddServiceLevel(CatalogueItemId solutionId)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var model = new AddEditServiceLevelModel(catalogueItem)
            {
                BackLink = Url.Action(nameof(EditServiceLevelAgreement), new { solutionId }),
            };

            return View("AddEditServiceLevel", model);
        }

        [HttpPost("add-service-level")]
        public async Task<IActionResult> AddServiceLevel(CatalogueItemId solutionId, AddEditServiceLevelModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEditServiceLevel", model);

            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var addServiceLevelModel = new EditServiceLevelModel
            {
                ServiceType = model.ServiceType,
                ServiceLevel = model.ServiceLevel,
                HowMeasured = model.HowMeasured,
                CreditsApplied = model.CreditsApplied!.Value,
            };

            await serviceLevelAgreementsService.AddServiceLevel(solutionId, addServiceLevelModel);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("edit-service-level/{serviceLevelId}")]
        public async Task<IActionResult> EditServiceLevel(CatalogueItemId solutionId, int serviceLevelId)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceLevel = catalogueItem.Solution.ServiceLevelAgreement.ServiceLevels.FirstOrDefault(sl => sl.Id == serviceLevelId);
            if (serviceLevel is null)
                return BadRequest($"No Service Level found for Id: {serviceLevelId}");

            var model = new AddEditServiceLevelModel(catalogueItem, serviceLevel)
            {
                BackLink = Url.Action(nameof(EditServiceLevelAgreement), new { solutionId }),
                CanDelete = catalogueItem.PublishedStatus != PublicationStatus.Published || catalogueItem.Solution.ServiceLevelAgreement.ServiceLevels.Any(sl => sl.Id != serviceLevelId),
            };

            return View("AddEditServiceLevel", model);
        }

        [HttpPost("edit-service-level/{serviceLevelId}")]
        public async Task<IActionResult> EditServiceLevel(CatalogueItemId solutionId, int serviceLevelId, AddEditServiceLevelModel model)
        {
            if (!ModelState.IsValid)
                return View("AddEditServiceLevel", model);

            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            if (!catalogueItem.Solution.ServiceLevelAgreement.ServiceLevels.Any(sl => sl.Id == serviceLevelId))
                return BadRequest($"No Service Level found for Id: {serviceLevelId}");

            var addServiceLevelModel = new EditServiceLevelModel
            {
                ServiceType = model.ServiceType,
                ServiceLevel = model.ServiceLevel,
                HowMeasured = model.HowMeasured,
                CreditsApplied = model.CreditsApplied!.Value,
            };

            await serviceLevelAgreementsService.UpdateServiceLevel(solutionId, serviceLevelId, addServiceLevelModel);

            return RedirectToAction(nameof(Index), new { solutionId });
        }

        [HttpGet("delete-service-level/{serviceLevelId}")]
        public async Task<IActionResult> DeleteServiceLevel(CatalogueItemId solutionId, int serviceLevelId)
        {
            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceLevel = catalogueItem.Solution.ServiceLevelAgreement.ServiceLevels.FirstOrDefault(sl => sl.Id == serviceLevelId);
            if (serviceLevel is null)
                return BadRequest($"No Service Level found for Id: {serviceLevelId}");

            var model = new DeleteServiceLevelModel(catalogueItem, serviceLevel)
            {
                BackLink = Url.Action(nameof(EditServiceLevel), new { solutionId, serviceLevelId }),
            };

            return View(model);
        }

        [HttpPost("delete-service-level/{serviceLevelId}")]
        public async Task<IActionResult> DeleteServiceLevel(CatalogueItemId solutionId, int serviceLevelId, DeleteServiceLevelModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var catalogueItem = await solutionsService.GetSolutionWithServiceLevelAgreements(solutionId);
            if (catalogueItem is null)
                return BadRequest($"No Solution found for Id: {solutionId}");

            var serviceLevel = catalogueItem.Solution.ServiceLevelAgreement.ServiceLevels.FirstOrDefault(sl => sl.Id == serviceLevelId);
            if (serviceLevel is null)
                return BadRequest($"No Service Level found for Id: {serviceLevelId}");

            await serviceLevelAgreementsService.DeleteServiceLevel(solutionId, serviceLevelId);

            return RedirectToAction(nameof(Index), new { solutionId });
        }
    }
}
