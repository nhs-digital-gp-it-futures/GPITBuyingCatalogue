using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models
{
    public class CheckBoxModel
    {
        public bool SingleCheckBoxProperty { get; set; }

        public bool AnotherCheckBoxProperty { get; set; }

        public bool EmbeddedCheckBoxProperty { get; set; }

        public string ForInput { get; set; }

        public List<CheckBoxListObject> ListOfObjects { get; set; }

        public class CheckBoxListObject
        {
            public string Name { get; set; }

            public bool Checked { get; set; }
        }
    }
}
