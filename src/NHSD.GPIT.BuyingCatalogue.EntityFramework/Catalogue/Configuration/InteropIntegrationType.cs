using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public enum InteropIntegrationType
{
    [EnumMember(Value = "IM1")]
    Im1,
    [EnumMember(Value = "GP Connect")]
    GpConnect,
    [EnumMember(Value = "NHS App")]
    NhsApp,
}
