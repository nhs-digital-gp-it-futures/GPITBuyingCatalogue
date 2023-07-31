using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class NonPriceElements : IAudited
{
    public int Id { get; set; }

    public int CompetitionId { get; set; }

    public int? ImplementationId { get; set; }

    public int? ServiceLevelId { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime LastUpdated { get; set; }

    public Competition Competition { get; set; }

    public ImplementationCriteria Implementation { get; set; }

    public ServiceLevelCriteria ServiceLevel { get; set; }

    public NonPriceWeights NonPriceWeights { get; set; }

    public ICollection<InteroperabilityCriteria> Interoperability { get; set; } =
        new HashSet<InteroperabilityCriteria>();
}
