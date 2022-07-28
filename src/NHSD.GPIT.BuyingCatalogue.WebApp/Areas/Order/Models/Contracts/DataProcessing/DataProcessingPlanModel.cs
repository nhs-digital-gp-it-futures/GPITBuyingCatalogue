using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DataProcessing;

public class DataProcessingPlanModel : NavBaseModel
{
    public const string NoOptionText = "No, I've agreed variations to the default data processing information with the supplier";

    public DataProcessingPlanModel()
    {
    }

    public DataProcessingPlanModel(ContractFlags contract)
    {
        UseDefaultDataProcessing = contract?.UseDefaultDataProcessing;
    }

    public CallOffId CallOffId { get; set; }

    public bool? UseDefaultDataProcessing { get; set; }

    public List<SelectableRadioOption<bool>> AvailableDataProcessingOptions => new()
    {
        new("Yes", true),
        new(NoOptionText, false),
    };
}
