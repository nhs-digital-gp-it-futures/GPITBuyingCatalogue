using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;

public class AddEditFrameworkModel : NavBaseModel
{
    public AddEditFrameworkModel()
    {
    }

    public AddEditFrameworkModel(EntityFramework.Catalogue.Models.Framework framework)
    {
        FrameworkId = framework.Id;
        Name = framework.ShortName;

        foreach (FundingType i in framework.FundingTypes)
        {
            FundingTypes.Where(x => x.Value == i).FirstOrDefault().Selected = true;
        }
    }

    public string FrameworkId { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    public List<SelectOption<FundingType>> FundingTypes { get; set; } = new()
    {
        new SelectOption<FundingType>(FundingType.Gpit.Description(), FundingType.Gpit),
        new SelectOption<FundingType>(FundingType.LocalFunding.Description(), FundingType.LocalFunding),
        new SelectOption<FundingType>(FundingType.Pcarp.Description(), FundingType.Pcarp),
    };

    public List<SelectOption<bool>> FoundationSolutionOptions => new List<SelectOption<bool>>
        {
            new("Yes", true),
            new("No", false),
        };

    public bool SupportsFoundationSolution { get; set; }
}
