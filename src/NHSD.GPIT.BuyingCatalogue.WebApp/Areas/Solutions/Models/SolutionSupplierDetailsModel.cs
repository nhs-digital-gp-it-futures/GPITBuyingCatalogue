using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionSupplierDetailsModel : SolutionDisplayBaseModel
    {
        public override int Index => 12;

        public string Summary { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public List<SupplierContactViewModel> Contacts { get; set; }
    }
}
