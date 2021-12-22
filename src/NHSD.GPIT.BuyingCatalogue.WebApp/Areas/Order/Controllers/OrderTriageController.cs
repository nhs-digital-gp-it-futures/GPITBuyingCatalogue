using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/triage")]
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

        [HttpGet]
        public IActionResult Index(string odsCode, TriageOption? option = null)
        {
            var model = new OrderTriageModel
            {
                BackLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { odsCode }),
                SelectedTriageOption = option,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(string odsCode, OrderTriageModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.SelectedTriageOption == TriageOption.NotSure)
                return RedirectToAction(nameof(NotSure), new { odsCode });

            return RedirectToAction(nameof(TriageSelection), new { odsCode, option = model.SelectedTriageOption });
        }

        [HttpGet("not-sure")]
        public IActionResult NotSure(string odsCode)
        {
            var model = new GenericOrderTriageModel
            {
                BackLink = Url.Action(nameof(Index), new { odsCode, option = TriageOption.NotSure }),
                OrdersDashboardLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { odsCode }),
            };

            return View(model);
        }

        [HttpGet("{option}")]
        public IActionResult TriageSelection(string odsCode, TriageOption option, bool? selected = null)
        {
            var (title, advice, _) = GetTriageSelectionContent(option);

            var model = new TriageDueDiligenceModel
            {
                BackLink = Url.Action(nameof(Index), new { odsCode, option }),
                Advice = advice,
                Title = title,
                Selected = selected,
            };

            return View(model);
        }

        [HttpPost("{option}")]
        public IActionResult TriageSelection(string odsCode, TriageOption option, TriageDueDiligenceModel model)
        {
            if (!model.Selected.HasValue)
            {
                var (_, _, validationError) = GetTriageSelectionContent(option);
                ModelState.AddModelError(nameof(model.Selected), validationError);
            }

            if (!ModelState.IsValid)
                return View(model);

            if (!model.Selected.GetValueOrDefault())
                return RedirectToAction(nameof(StepsNotCompleted), new { odsCode, option });

            return RedirectToAction(
                nameof(OrderController.ReadyToStart),
                typeof(OrderController).ControllerName(),
                new { odsCode, option });
        }

        [HttpGet("{option}/steps-incomplete")]
        public IActionResult StepsNotCompleted(string odsCode, TriageOption option)
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
                BackLink = Url.Action(nameof(TriageSelection), new { odsCode, option, selected = false }),
                OrdersDashboardLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { odsCode }),
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
