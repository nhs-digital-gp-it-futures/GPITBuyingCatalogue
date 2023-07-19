using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public enum NonPriceElement
{
    [EnumMember(Value = "Implementation")]
    Implementation,
    [EnumMember(Value = "Interoperability")]
    Interoperability,
    [EnumMember(Value = "Service level")]
    ServiceLevel,
}
