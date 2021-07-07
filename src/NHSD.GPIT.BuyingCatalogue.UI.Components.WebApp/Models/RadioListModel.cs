using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models
{
    public class RadioListModel
    {
        public string SelectedItem { get; set; }

        public List<RadioListOptions> ListOptions { get; set; }

        public List<RadioListOptions> ConditionalOptions { get; set; }

        public string RadioItemYesNo { get; set; }

        public class RadioListOptions
        {
            public string Value { get; set; }

            public string Name { get; set; }
        }
    }
}
