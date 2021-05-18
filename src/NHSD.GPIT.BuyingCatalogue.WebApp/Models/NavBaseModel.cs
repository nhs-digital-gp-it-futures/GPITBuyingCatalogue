using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class NavBaseModel
    {
        public string BackLink { get; set; } = "./";

        public string BackLinkText { get; set; } = "Go back to previous page";

        protected void ProcessCheckboxFields(HashSet<string> fieldinput)
        {
            var checkboxProperties = GetType().GetProperties().Where(prop => prop.IsDefined(typeof(CheckboxAttribute)));

            foreach (var prop in checkboxProperties)
            {
                prop.SetValue(this, fieldinput.Contains(prop.GetCustomAttribute<CheckboxAttribute>().FieldText));
            }
        }
    }
}
