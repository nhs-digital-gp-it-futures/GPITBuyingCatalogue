﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public enum OrderItemFundingType
    {
        [Description("None specified")]
        [Display(Name = nameof(None), Order = 0)]
        None = 0,

        [Description("Central")]
        [Display(Name = nameof(CentralFunding), Order = 1)]
        CentralFunding = 1,

        [Description("Local")]
        [Display(Name = nameof(LocalFunding), Order = 2)]
        LocalFunding = 2,

        [Description("Mixed")]
        [Display(Name = nameof(MixedFunding), Order = 3)]
        MixedFunding = 3,

        [Description("None required")]
        [Display(Name = nameof(NoFundingRequired), Order = 4)]
        NoFundingRequired = 4,

        [Description("Local only")]
        [Display(Name = nameof(LocalFundingOnly), Order = 5)]
        LocalFundingOnly = 5,
    }
}
