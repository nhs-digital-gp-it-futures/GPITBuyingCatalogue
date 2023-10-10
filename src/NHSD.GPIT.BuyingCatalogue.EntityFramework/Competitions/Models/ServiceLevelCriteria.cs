using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class ServiceLevelCriteria
{
    public int Id { get; set; }

    public DateTime TimeFrom { get; set; }

    public DateTime TimeUntil { get; set; }

    public ICollection<Iso8601DayOfWeek> ApplicableDays { get; set; }

    public bool IncludesBankHolidays { get; set; }

    public int NonPriceElementsId { get; set; }
}
