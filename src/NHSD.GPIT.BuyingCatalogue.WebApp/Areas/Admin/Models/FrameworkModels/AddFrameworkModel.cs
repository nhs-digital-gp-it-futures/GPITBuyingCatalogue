using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;

public class AddFrameworkModel : NavBaseModel
{
    public string FrameworkId { get; set; }

    public string Name { get; set; }

    public bool? IsLocalFundingOnly { get; set; }

    public IEnumerable<SelectOption<bool>> FundingTypes => new[]
    {
        new SelectOption<bool>("Local funding", true), new SelectOption<bool>("Central funding", false),
    };
}
