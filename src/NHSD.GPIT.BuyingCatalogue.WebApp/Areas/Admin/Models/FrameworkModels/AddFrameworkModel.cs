using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;

public class AddFrameworkModel : NavBaseModel
{
    public string FrameworkId { get; set; }

    public string Name { get; set; }

    public List<SelectOption<FundingType>> FundingTypes { get; set; } = new()
    {
        new SelectOption<FundingType>(FundingType.Gpit.Description(), FundingType.Gpit),
        new SelectOption<FundingType>(FundingType.LocalFunding.Description(), FundingType.LocalFunding),
        new SelectOption<FundingType>(FundingType.Pcarp.Description(), FundingType.Pcarp),
    };
}
