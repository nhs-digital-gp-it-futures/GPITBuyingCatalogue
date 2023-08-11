using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Completed")]
        Completed = 1,

        [EnumMember(Value = "In progress")]
        InProgress = 2,

        [EnumMember(Value = "Deleted")]
        Deleted = 3,

        [EnumMember(Value = "Terminated")]
        Terminated = 4,
    }
}
