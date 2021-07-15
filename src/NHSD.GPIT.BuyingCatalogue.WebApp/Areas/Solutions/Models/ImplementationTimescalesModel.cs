using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class ImplementationTimescalesModel : SolutionDisplayBaseModel
    {
        public string Description { get; set; }

        public override int Index => 7;
    }
}
