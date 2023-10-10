using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class NonPriceElements
{
    public int Id { get; set; }

    public int CompetitionId { get; set; }

    public Competition Competition { get; set; }

    public ImplementationCriteria Implementation { get; set; }

    public ServiceLevelCriteria ServiceLevel { get; set; }

    public NonPriceWeights NonPriceWeights { get; set; }

    public ICollection<InteroperabilityCriteria> Interoperability { get; set; } =
        new HashSet<InteroperabilityCriteria>();

    public ICollection<FeaturesCriteria> Features { get; set; } =
        new HashSet<FeaturesCriteria>();
}
