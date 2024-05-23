using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

public class CompetitionContractModel : NavBaseModel
{
    public CompetitionContractModel()
    {
    }

    public CompetitionContractModel(
        Competition competition)
    {
        ArgumentNullException.ThrowIfNull(competition);
        ArgumentNullException.ThrowIfNull(competition.Framework);

        CompetitionName = competition.Name;
        ContractLength = competition.ContractLength;
        ContractLengthLimit = competition.Framework.MaximumTerm;
    }

    public string CompetitionName { get; set; }

    public int ContractLengthLimit { get; set; }

    [Description("Contract length")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? ContractLength { get; set; }
}
