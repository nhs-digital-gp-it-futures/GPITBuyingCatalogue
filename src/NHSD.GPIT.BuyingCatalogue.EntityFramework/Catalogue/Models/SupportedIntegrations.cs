using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public enum SupportedIntegrations
{
    [EnumMember(Value = "IM1")]
    Im1 = 0,
    [EnumMember(Value = "GP Connect")]
    GpConnect = 1,
    [EnumMember(Value = "NHS App")]
    NhsApp = 2,
}
