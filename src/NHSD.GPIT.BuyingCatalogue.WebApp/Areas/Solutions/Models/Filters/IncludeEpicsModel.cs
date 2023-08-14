using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class IncludeEpicsModel
    {
        public const string YesRadioOption = "Filter by Epics";
        public const string NoRadioOption = "Go to results";

        public string Selected { get; set; }

        public bool? IncludeEpics { get; set; }

        public IEnumerable<SelectOption<bool>> Options => new List<SelectOption<bool>>
        {
            new(YesRadioOption, true),
            new(NoRadioOption, false),
        };
    }
}
