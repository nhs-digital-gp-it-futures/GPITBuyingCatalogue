using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models;
using static NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models.RadioListModel;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers
{
    public sealed class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("action-link")]
        public IActionResult ActionLink()
        {
            return View(new BlankModel());
        }

        [HttpGet("address")]
        public IActionResult Address()
        {
            var address = new Address
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

        [HttpGet("breadcrumbs")]
        public IActionResult Breadcrumbs()
        {
            var model = new BreadcrumbsModel
            {
                Breadcrumbs = new Dictionary<string, string>
                {
                    { "Action Links", "action-links" },
                    { "Address", "address" },
                },
            };

            return View(model);
        }

        [HttpGet("care-card")]
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

        [HttpGet("buttons")]
        public IActionResult Buttons()
        {
            return View(new ButtonsModel());
        }

        [HttpGet("do-and-dont-list")]
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

        [HttpGet("images")]
        public IActionResult Images()
        {
            return View(new BlankModel());
        }

        [HttpGet("checkboxes")]
        public IActionResult Checkboxes()
        {
            var model = new CheckBoxModel
            {
                ListOfObjects = new List<CheckBoxModel.CheckBoxListObject>
                {
                    new() { Name = "First Checkbox" },
                    new() { Name = "Second Checkbox" },
                    new() { Name = "Third Checkbox" },
                },
            };

            return View(model);
        }

        [HttpGet("page-title")]
        public IActionResult PageTitle()
        {
            return View(new BlankModel());
        }

        [HttpGet("date-input")]
        public IActionResult DateInput()
        {
            return View(new DateInputModel());
        }

        [HttpGet("time-input")]
        public IActionResult TimeInput()
        {
            return View(new TimeInputModel());
        }

        [HttpGet("details-and-expanders")]
        public IActionResult DetailsAndExpanders()
        {
            return View(new BlankModel());
        }

        [HttpGet("end-note")]
        public IActionResult EndNote()
        {
            return View(new BlankModel());
        }

        [HttpGet("fieldsets")]
        public IActionResult FieldSets()
        {
            return View(new BlankModel());
        }

        [HttpGet("text-input")]
        public IActionResult TextInput()
        {
            return View(new TextInputModel());
        }

        [HttpGet("bookended-text-input")]
        public IActionResult BookendedTextInput()
        {
            return View(new TextInputModel());
        }

        [HttpGet("text-area")]
        public IActionResult TextArea()
        {
            return View(new TextInputModel());
        }

        [HttpGet("inset-text")]
        public IActionResult InsetText()
        {
            return View(new BlankModel());
        }

        [HttpGet("radio-lists")]
        public IActionResult RadioLists()
        {
            var model = new RadioListModel
            {
                ListOptions = new List<RadioListOptions>(3)
                {
                    new() { Name = "First Option", Value = "1" },
                    new() { Name = "Second Option", Value = "2" },
                    new() { Name = "Third Option", Value = "3" },
                },
                ConditionalOptions = new List<RadioListOptions>(2)
                {
                    new() { Name = "This Option Contains a Text Input", Value = "1" },
                    new() { Name = "This Option Contains some Text", Value = "2" },
                },
            };

            return View(model);
        }

        [HttpGet("yes-no-radios")]
        public IActionResult YesNoRadios()
        {
            return View(new RadioListModel());
        }

        [HttpGet("select-list")]
        public IActionResult SelectList()
        {
            var model = new SelectListModel
            {
                SelectListItems = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
                {
                    new("First Option", "1"),
                    new("Second Option", "2"),
                    new("Third Option", "3"),
                },
            };

            return View(model);
        }

        [HttpGet("summary-list")]
        public IActionResult SummaryList()
        {
            return View(new BlankModel());
        }

        [HttpGet("table")]
        public IActionResult Table()
        {
            var model = new TableModel
            {
                Rows = new List<TableModel.TableRowModel>
                {
                    new()
                    {
                        FirstColumn = "First Column",
                        SecondColumn = "Second Column",
                        ThirdColumn = "Third Column",
                        FourthColumn = "Fourth Column",
                        FifthColumn = "Fifth Column",
                        SixthColumn = "Sixth Column",
                        SeventhColumn = "Seventh Column",
                    },
                    new()
                    {
                        FirstColumn = "First Column",
                        SecondColumn = "Second Column",
                        ThirdColumn = string.Empty,
                        FourthColumn = "Fourth Column",
                        FifthColumn = "Fifth Column",
                        SixthColumn = "Sixth Column",
                        SeventhColumn = "Seventh Column",
                    },
                    new()
                    {
                        FirstColumn = "First Column",
                        SecondColumn = "Second Column",
                        ThirdColumn = "Third Column",
                        FourthColumn = "Fourth Column",
                        FifthColumn = "Fifth Column",
                        SixthColumn = "Sixth Column",
                        SeventhColumn = string.Empty,
                    },
                },
            };

            return View(model);
        }

        [HttpGet("tags")]
        public IActionResult Tags()
        {
            return View(new BlankModel());
        }

        [HttpGet("validation-summary")]
        public IActionResult ValidationSummary()
        {
            const string error = "This Input is in Error.";
            const string secondError = "I am an Input in Error at the bottom of the Page.";

            ModelState.AddModelError("ARandomField", error);
            ModelState.AddModelError("SecondRandomField", secondError);

            return View(new BlankModel());
        }

        [HttpGet("warning-callout")]
        public IActionResult WarningCallout()
        {
            return View(new BlankModel());
        }
    }
}
