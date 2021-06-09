using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Views
{
    public class ComponentTestModel : PageModel
    {
        public ComponentTestModel()
        {
        }

        public Dictionary<string, string> Breadcrumbs
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "Level One", "some/url" },
                    { "Level Two", "another/url" },
                };
            }
        }

        public List<string> ListOfStringOptions
        {
            get
            {
                return new List<string>
                {
                    "this is the first option",
                    "this is the second option",
                    "this is the third option",
                };
            }
        }

        public string BookendedInputText { get; set; }

        public string SecondBookendedInput { get; set; }

        public string ThirdBookenedInput { get; set; }

        public string UnpostedInputText { get; set; }

        [StringLength(100)]
        public string InputText { get; set; }

        [StringLength(99)]
        public string InputTextSecond { get; set; }

        [StringLength(50)]
        [Password]
        public string InputTextPassword { get; set; }

        [StringLength(200)]
        public string TextAreaText { get; set; }

        public string TextAreaTextSecond { get; set; }

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public bool CheckBox { get; set; }

        public CheckStuff SecondCheckBox { get; } = new CheckStuff { Name = "CheckBoxName" };

        public List<CheckStuffValue> RadioFromList { get; }
            = new List<CheckStuffValue>
            {
            new CheckStuffValue { Name = "first item", Value = "first" },
            new CheckStuffValue { Name = "second item", Value = "second" },
            new CheckStuffValue { Name = "third item", Value = "third" },
            };

        public string SelectedRadio { get; set; }

        public bool YesNoRadio { get; set; }

        public List<SelectListItem> SelectListItems { get; }
            = new List<SelectListItem>
            {
                new SelectListItem("first item", "1"),
                new SelectListItem("second item", "2"),
                new SelectListItem("third item", "3"),
            };

        public string SelectedItemForSelectList { get; set; }

        public string FormLine1 { get; set; }

        public string FormLine2 { get; set; }

        public string FormLine3 { get; set; }

        public string FormLine4 { get; set; }

        public EntityFramework.Models.Ordering.Address Adress
        {
            get
            {
                return new EntityFramework.Models.Ordering.Address
                {
                    Line1 = "Our House",
                    Line2 = "In The Middle",
                    Line3 = "OfOurStreet",
                    Town = "In a Town",
                    Country = "In a Country",
                    Postcode = "P05T CD3",
                };
            }
        }

        public List<TableRowValues> TableRows
            {
                get
                {
                return new List<TableRowValues>
                    {
                        new TableRowValues("first", "second", "third", "fourth", "fifth", "sixth"),
                        new TableRowValues("some more values", "that go here", string.Empty, "and here", "not there", "aaa"),
                        new TableRowValues("first", "second", "third", "fourth", "fifth", string.Empty),
                    };
                }
            }

        public (DateTime? Date, string Error) ToDateTime()
        {
            try
            {
                var date = DateTime.Parse($"{Day}/{Month}/{Year}");

                if (date.ToUniversalTime() <= DateTime.UtcNow.AddDays(-60))
                    return (null, "Commencement date must be in the future or within the last 60 days");

                return (date, null);
            }
            catch (FormatException)
            {
                return (null, "Commencement date must be a real date");
            }
        }

        public class CheckStuff
        {
            public string Name { get; set; }

            public bool Checked { get; set; }
        }

        public class CheckStuffValue
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        public class TableRowValues
        {
            public TableRowValues(string col1, string col2, string col3, string col4, string col5, string col6)
            {
                Col1 = col1;
                Col2 = col2;
                Col3 = col3;
                Col4 = col4;
                Col5 = col5;
                Col6 = col6;
            }

            public string Col1 { get; set; }

            public string Col2 { get; set; }

            public string Col3 { get; set; }

            public string Col4 { get; set; }

            public string Col5 { get; set; }

            public string Col6 { get; set; }
        }
    }
}
