using System.ComponentModel;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public enum FundingType
{
    [Description("GPIT funding")]
    Gpit = 1,
    [Description("Local funding")]
    LocalFunding = 2,
    [Description("PCARP funding")]
    Pcarp = 3,
}
