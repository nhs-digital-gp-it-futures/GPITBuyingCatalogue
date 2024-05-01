using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.FrameworkModels;

public class AddEditFrameworkModel : NavBaseModel
{
    public static readonly PageTitleModel AddPageTitle = new()
    {
        Title = "Add a framework",
        Advice = "Provide details for this framework.",
    };

    public static readonly PageTitleModel EditPageTitle = new()
    {
        Title = "Edit framework",
        Advice = "These are the current details for this framework.",
    };

    public AddEditFrameworkModel()
    {
    }

    public AddEditFrameworkModel(EntityFramework.Catalogue.Models.Framework framework)
    {
        FrameworkId = framework.Id;
        Name = framework.ShortName;
        MaximumTerm = framework.MaximumTerm.ToString();

        foreach (FundingType i in framework.FundingTypes)
        {
            FundingTypes.Where(x => x.Value == i).FirstOrDefault().Selected = true;
        }
    }

    public string FrameworkId { get; set; }

    [StringLength(100)]
    public string Name { get; set; }

    public string MaximumTerm { get; set; }

    public List<SelectOption<FundingType>> FundingTypes { get; set; } = new()
    {
        new SelectOption<FundingType>(FundingType.Gpit.Description(), FundingType.Gpit),
        new SelectOption<FundingType>(FundingType.LocalFunding.Description(), FundingType.LocalFunding),
        new SelectOption<FundingType>(FundingType.Pcarp.Description(), FundingType.Pcarp),
    };

    public PageTitleModel GetPageTitle()
    {
        return FrameworkId is null
            ? AddPageTitle
            : EditPageTitle;
    }
}
