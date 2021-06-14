using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models
{
    public class RadioListModel
    {
        public string SelectedItem { get; set; }

        public List<RadioListOptions> Options { get; set; }

        public string RadioItemYesNo { get; set; }

        public class RadioListOptions
        {
            public string Value { get; set; }

            public string Name { get; set; }
        }
    }
}
