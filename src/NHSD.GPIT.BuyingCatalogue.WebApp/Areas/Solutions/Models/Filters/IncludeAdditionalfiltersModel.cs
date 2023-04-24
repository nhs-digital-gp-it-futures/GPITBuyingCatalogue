using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    [ExcludeFromCodeCoverage]
    public class IncludeAdditionalfiltersModel
    {
        public IncludeAdditionalfiltersModel()
        {
        }

        public IncludeAdditionalfiltersModel(List<EntityFramework.Catalogue.Models.Framework> frameworks)
        {
            FrameworkOptions = frameworks.Select(f => new SelectOption<string>
            {
                Value = f.Id,
                Text = f.ShortName,
                Selected = false,
            }).ToList();
        }

        public string SelectedFrameworkId { get; set; }

        public List<SelectOption<string>> FrameworkOptions { get; set; }
    }
}
