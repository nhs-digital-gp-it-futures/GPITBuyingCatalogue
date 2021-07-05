using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionSupplierDetailsModel : SolutionDisplayBaseModel
    {
        public override int Index => 12;

        public string Summary { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public List<SupplierContactViewModel> Contacts { get; set; }

        public bool HasContacts() => Contacts?.Any(c => c != null) == true;
    }
}
