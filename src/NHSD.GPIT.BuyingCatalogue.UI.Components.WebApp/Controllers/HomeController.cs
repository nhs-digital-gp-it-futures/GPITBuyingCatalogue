using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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

        [Route("action-link")]
        public IActionResult ActionLink()
        {
            return View(new BlankModel());
        }

        [Route("address")]
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

        [Route("breadcrumbs")]
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
                    new() { Name = "First Checkbox" },
                    new() { Name = "Second Checkbox" },
                    new() { Name = "Third Checkbox" },
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

        [Route("inset-text")]
        public IActionResult InsetText()
        {
            return View(new BlankModel());
        }

        [Route("radio-lists")]
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
                    new("First Option", "1"),
                    new("Second Option", "2"),
                    new("Third Option", "3"),
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

        [Route("tags")]
        public IActionResult Tags()
        {
            return View(new BlankModel());
        }

        [Route("validation-summary")]
        public IActionResult ValidationSummary()
        {
            const string error = "This Input is in Error.";
            const string secondError = "I am an Input in Error at the bottom of the Page.";

            ModelState.AddModelError("ARandomField", error);
            ModelState.AddModelError("SecondRandomField", secondError);

            return View(new BlankModel());
        }

        [Route("warning-callout")]
        public IActionResult WarningCallout()
        {
            return View(new BlankModel());
        }
    }
}
