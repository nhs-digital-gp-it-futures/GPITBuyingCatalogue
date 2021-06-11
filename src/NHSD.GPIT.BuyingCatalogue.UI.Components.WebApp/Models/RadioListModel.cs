using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models
{
    public class RadioListModel
    {
        public string SelectedItem { get; set; }

        public List<string> Options { get; set; }

        public string RadioItemYesNo { get; set; }
    }
}
