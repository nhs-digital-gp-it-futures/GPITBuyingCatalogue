using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/triage")]
    public class OrderTriageController : Controller
    {
        private static readonly Dictionary<TriageOption, (string Title, string Advice, string ValidationError)> TriageSelectionContent = new()
        {
            [TriageOption.Under40k] = ("Have you identified what you want to order?",
                   "As your order is under £40k, you should have executed a Direct Award. You can procure any Catalogue Solution or service on the Buying Catalogue without carrying out a competition.",
                   "Select yes if you’ve identified what you want to order"),
            [TriageOption.Between40kTo250k] = ("Have you carried out a competition using the Buying Catalogue?",
                   "As your order is between £40k and £250k, you should have executed an On-Catalogue Competition to identify the Catalogue Solution that best meets your needs.",
                   "Select yes if you’ve carried out a competition on the Buying Catalogue"),
            [TriageOption.Over250k] = ("Have you sent out Invitations to Tender to suppliers?",
                   "As your order is over £250k, you should have executed an Off-Catalogue Competition to identify the Catalogue Solution that best meets your needs.",
                   "Select yes if you’ve carried out an Off-Catalogue Competition with suppliers"),
        };

        private readonly IOrganisationsService organisationsService;

        public OrderTriageController(IOrganisationsService organisationsService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        }

        [HttpGet("proxy-select")]
        public async Task<IActionResult> SelectOrganisation(string internalOrgId, TriageOption? option = null)
        {
            if (!User.GetSecondaryOrganisationInternalIdentifiers().Any())
                return RedirectToAction(nameof(Index), new { internalOrgId, option });

            var internalOrgIds = new List<string>(User.GetSecondaryOrganisationInternalIdentifiers())
            {
                User.GetPrimaryOrganisationInternalIdentifier(),
            };

            var organisations = await organisationsService.GetOrganisationsByInternalIdentifiers(internalOrgIds.ToArray());

            var model = new SelectOrganisationModel(internalOrgId, organisations)
            {
                BackLink = Url.Action(nameof(DashboardController.Organisation), typeof(DashboardController).ControllerName(), new { internalOrgId, option }),
                Title = "Which organisation are you ordering for?",
            };

            return View(model);
        }

        [HttpPost("proxy-select")]
        public IActionResult SelectOrganisation(string internalOrgId, SelectOrganisationModel model, TriageOption? option = null)
        {
            if (!User.GetSecondaryOrganisationInternalIdentifiers().Any())
                return RedirectToAction(nameof(Index), new { internalOrgId, option });

            if (!ModelState.IsValid)
                return View(model);

            if (!string.Equals(internalOrgId, model.SelectedOrganisation, StringComparison.OrdinalIgnoreCase))
                option = null;

            return RedirectToAction(nameof(Index), new { internalOrgId = model.SelectedOrganisation, option });
        }

        [HttpGet]
        public IActionResult Index(string internalOrgId, TriageOption? option = null)
        {
            var backlink = User.GetSecondaryOrganisationInternalIdentifiers().Any()
                ? Url.Action(
                    nameof(SelectOrganisation),
                    new { internalOrgId, option })
                : Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId });

            var model = new OrderTriageModel
            {
                BackLink = backlink,
                SelectedTriageOption = option,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(string internalOrgId, OrderTriageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.SelectedTriageOption == TriageOption.NotSure)
                return RedirectToAction(nameof(NotSure), new { internalOrgId });

            return RedirectToAction(nameof(TriageSelection), new { internalOrgId, option = model.SelectedTriageOption });
        }

        [HttpGet("not-sure")]
        public IActionResult NotSure(string internalOrgId)
        {
            var model = new GenericOrderTriageModel
            {
                BackLink = Url.Action(nameof(Index), new { internalOrgId, option = TriageOption.NotSure }),
                InternalOrgId = internalOrgId,
                OrdersDashboardLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId }),
            };

            return View(model);
        }

        [HttpGet("{option}")]
        public IActionResult TriageSelection(string internalOrgId, TriageOption? option, bool? selected = null)
        {
            if (option is null)
                return RedirectToAction(nameof(Index), new { internalOrgId });

            var (title, advice, _) = GetTriageSelectionContent(option!.Value);

            var model = new TriageDueDiligenceModel
            {
                BackLink = Url.Action(nameof(Index), new { internalOrgId, option }),
                Advice = advice,
                Title = title,
                Selected = selected,
            };

            return View(model);
        }

        [HttpPost("{option}")]
        public IActionResult TriageSelection(string internalOrgId, TriageOption option, TriageDueDiligenceModel model)
        {
            if (!model.Selected.HasValue)
            {
                var (_, _, validationError) = GetTriageSelectionContent(option);
                ModelState.AddModelError(nameof(model.Selected), validationError);
            }

            if (!ModelState.IsValid)
                return View(model);

            if (!model.Selected.GetValueOrDefault())
                return RedirectToAction(nameof(StepsNotCompleted), new { internalOrgId, option });

            return RedirectToAction(
                nameof(OrderController.ReadyToStart),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, option });
        }

        [HttpGet("{option}/steps-incomplete")]
        public IActionResult StepsNotCompleted(string internalOrgId, TriageOption option)
        {
            var viewName = option switch
            {
                TriageOption.Under40k => "Incomplete40k",
                TriageOption.Between40kTo250k => "Incomplete40kTo250k",
                TriageOption.Over250k => "IncompleteOver250k",
                _ => throw new KeyNotFoundException(),
            };

            var model = new GenericOrderTriageModel
            {
                BackLink = Url.Action(nameof(TriageSelection), new { internalOrgId, option, selected = false }),
                InternalOrgId = internalOrgId,
                OrdersDashboardLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId }),
            };

            return View(viewName, model);
        }

        private static (string Title, string Advice, string ValidationError) GetTriageSelectionContent(TriageOption option)
        {
            if (!TriageSelectionContent.TryGetValue(option, out var content))
                throw new KeyNotFoundException($"Key '{option}' is not a valid triage selection");

            return content;
        }
    }
}
