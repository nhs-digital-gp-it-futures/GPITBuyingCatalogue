using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

public class AspNetUserLoginEvent
{
    public AspNetUserLoginEvent()
    {
    }

    public AspNetUserLoginEvent(DateTime date)
    {
        Date = date;
    }

    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime Date { get; set; }

    public AspNetUser User { get; set; }
}
