using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

[ExcludeFromCodeCoverage]
public class AspNetUserEvent
{
    public AspNetUserEvent()
    {
    }

    public AspNetUserEvent(
        int eventTypeId)
    {
        EventTypeId = eventTypeId;
    }

    public int Id { get; set; }

    public int UserId { get; set; }

    public int EventTypeId { get; set; }

    public EventType EventType { get; set; }
}
