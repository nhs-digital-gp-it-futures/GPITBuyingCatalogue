﻿using System;
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
            return View(new BlankModel());
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
            return View(new BlankModel());
        }

        [Route("checkboxes")]
        public IActionResult Checkboxes()
        {
            var model = new CheckBoxModel
            {
                ListOfObjects = new List<CheckBoxModel.CheckBoxListObject>
                {
                    new CheckBoxModel.CheckBoxListObject { Name = "First Checkbox" },
                    new CheckBoxModel.CheckBoxListObject { Name = "Second Checkbox" },
                    new CheckBoxModel.CheckBoxListObject { Name = "Third Checkbox" },
                },
            };

            return View(model);
        }

        [Route("page-title")]
        public IActionResult PageTitle()
        {
            return View(new BlankModel());
        }

        [Route("date-input")]
        public IActionResult DateInput()
        {
            return View(new DateInputModel());
        }

        [Route("details-and-expanders")]
        public IActionResult DetailsAndExpanders()
        {
            return View(new BlankModel());
        }

        [Route("end-note")]
        public IActionResult EndNote()
        {
            return View(new BlankModel());
        }

        [Route("fieldsets")]
        public IActionResult FieldSets()
        {
            return View(new BlankModel());
        }

        [Route("text-input")]
        public IActionResult TextInput()
        {
            return View(new TextInputModel());
        }

        [Route("bookended-text-input")]
        public IActionResult BookendedTextInput()
        {
            return View(new TextInputModel());
        }

        [Route("text-area")]
        public IActionResult TextArea()
        {
            return View(new TextInputModel());
        }

        [Route("radio-lists")]
        public IActionResult RadioLists()
        {
            var model = new RadioListModel()
            {
                Options = new List<string>
                {
                    "First Option",
                    "Second Option",
                    "Third Option",
                },
            };

            return View(model);
        }

        [Route("yes-no-radios")]
        public IActionResult YesNoRadios()
        {
            return View(new RadioListModel());
        }

        [Route("select-list")]
        public IActionResult SelectList()
        {
            var model = new SelectListModel
            {
                SelectListItems = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("First Option", "1"),
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Second Option", "2"),
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Third Option", "3"),
                },
            };

            return View(model);
        }

        [Route("summary-list")]
        public IActionResult SummaryList()
        {
            return View(new BlankModel());
        }

        [Route("table")]
        public IActionResult Table()
        {
            return View(new BlankModel());
        }

        [Route("tags")]
        public IActionResult Tags()
        {
            return View(new BlankModel());
        }

        [Route("validation-summary")]
        public IActionResult ValidationSummary()
        {
            return View(new BlankModel());
        }

        [Route("warning-callout")]
        public IActionResult WarningCallout()
        {
            return View(new BlankModel());
        }
    }
}
