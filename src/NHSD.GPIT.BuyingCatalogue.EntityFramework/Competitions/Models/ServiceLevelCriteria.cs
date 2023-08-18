using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class ServiceLevelCriteria
{
    public int Id { get; set; }

    public DateTime TimeFrom { get; set; }

    public DateTime TimeUntil { get; set; }

    public string ApplicableDays { get; set; }

    public int NonPriceElementsId { get; set; }
}
