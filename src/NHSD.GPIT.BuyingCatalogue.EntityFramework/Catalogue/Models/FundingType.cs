using System.ComponentModel;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public enum FundingType
{
    [Description("GPIT funding")]
    GPIT = 1,
    [Description("Local funding")]
    Local = 2,
    [Description("PCARP funding")]
    PCARP = 3,
}
