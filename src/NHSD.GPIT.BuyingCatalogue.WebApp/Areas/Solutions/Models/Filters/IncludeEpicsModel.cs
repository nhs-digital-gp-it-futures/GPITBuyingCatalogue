using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class IncludeEpicsModel
    {
        public const string YesRadioOption = "Filter by Epics";
        public const string NoRadioOption = "Go to results";

        public string SelectedCapabilityIds { get; set; }

        public bool? IncludeEpics { get; set; }

        public IEnumerable<SelectableRadioOption<bool>> Options => new List<SelectableRadioOption<bool>>
        {
            new(YesRadioOption, true),
            new(NoRadioOption, false),
        };
    }
}
