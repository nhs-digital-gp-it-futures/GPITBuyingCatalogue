using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Complete")]
        Complete = 1,
        [EnumMember(Value = "In progress")]
        InProgress = 2,
    }
}
