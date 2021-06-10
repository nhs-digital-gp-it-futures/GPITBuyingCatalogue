using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("action-link")]
        public IActionResult ActionLink()
        {
            return View(new ActionLinkModel());
        }

        [Route("address")]
        public IActionResult Address()
        {
            var address = new EntityFramework.Models.Ordering.Address()
            {
                Line1 = "Our House",
                Line2 = "In The Middle",
                Line3 = "OfOurStreet",
                Town = "In a Town",
                Country = "In a Country",
                Postcode = "P05T CD3",
            };

            return View(address);
        }

        [Route("breadcrumbs")]
        public IActionResult Breadcrumbs()
        {
            var model = new BreadcrumbsModel()
            {
                Breadcrumbs = new Dictionary<string, string>
                {
                    { "Action Links", "action-links" },
                    { "Address", "address" },
                },
            };

            return View(model);
        }

        [Route("care-card")]
        public IActionResult CareCard()
        {
            var model = new CareCardModel
            {
                Options = new List<string>
                {
                    "this is the first option",
                    "this is the second option",
                    "this is the third option",
                },
            };

            return View(model);
        }

        [Route("buttons")]
        public IActionResult Buttons()
        {
            return View(new ButtonsModel());
        }

        [Route("do-and-dont-list")]
        public IActionResult DoAndDontList()
        {
            var model = new DoDontModel
            {
                Options = new List<string>
                {
                    "this is the first option",
                    "this is the second option",
                    "this is the third option",
                },
            };

            return View(model);
        }

        [Route("images")]
        public IActionResult Images()
        {
            return View(new ButtonsModel());
        }

        [Route("checkboxes")]
        public IActionResult Checkboxes()
        {
            return View(new ButtonsModel());
        }

        [Route("page-title")]
        public IActionResult PageTitle()
        {
            return View(new ButtonsModel());
        }

        [Route("date-input")]
        public IActionResult DateInput()
        {
            return View(new ButtonsModel());
        }

        [Route("details-and-expanders")]
        public IActionResult DetailsAndExpanders()
        {
            return View(new ButtonsModel());
        }

        [Route("end-note")]
        public IActionResult EndNote()
        {
            return View(new ButtonsModel());
        }

        [Route("fieldsets")]
        public IActionResult FieldSets()
        {
            return View(new ButtonsModel());
        }

        [Route("text-input")]
        public IActionResult TextInput()
        {
            return View(new ButtonsModel());
        }

        [Route("bookended-text-input")]
        public IActionResult BookendedTextInput()
        {
            return View(new ButtonsModel());
        }

        [Route("text-area")]
        public IActionResult TextArea()
        {
            return View(new ButtonsModel());
        }

        [Route("radio-lists")]
        public IActionResult RadioLists()
        {
            return View(new ButtonsModel());
        }

        [Route("yes-no-radios")]
        public IActionResult YesNoRadios()
        {
            return View(new ButtonsModel());
        }

        [Route("select-list")]
        public IActionResult SelectList()
        {
            return View(new ButtonsModel());
        }

        [Route("summary-list")]
        public IActionResult SummaryList()
        {
            return View(new ButtonsModel());
        }

        [Route("table")]
        public IActionResult Table()
        {
            return View(new ButtonsModel());
        }

        [Route("tags")]
        public IActionResult Tags()
        {
            return View(new ButtonsModel());
        }

        [Route("validation-summary")]
        public IActionResult ValidationSummary()
        {
            return View(new ButtonsModel());
        }

        [Route("warning-callout")]
        public IActionResult WarningCallout()
        {
            return View(new ButtonsModel());
        }
    }
}
