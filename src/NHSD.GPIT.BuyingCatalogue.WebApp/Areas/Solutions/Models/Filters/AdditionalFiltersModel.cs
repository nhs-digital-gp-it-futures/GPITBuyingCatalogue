using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class AdditionalFiltersModel
    {
        public List<SelectOption<string>> FrameworkOptions { get; set; }

        public string SelectedFrameworkIds { get; set; }

        public AdditionalFiltersModel()
        {
        }

        public AdditionalFiltersModel(List<EntityFramework.Catalogue.Models.Framework> frameworks)
        {
            //FrameworkOptions = new List<SelectOption<string, string>>();
            FrameworkOptions = frameworks.Select(f => new SelectOption<string>
            {
                Value = f.Id,
                Text = f.ShortName,
            }).ToList();
        }
    }
}
