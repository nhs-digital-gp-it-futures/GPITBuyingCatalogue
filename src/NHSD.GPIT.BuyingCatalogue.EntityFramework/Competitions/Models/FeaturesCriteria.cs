using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class FeaturesCriteria
{
    public FeaturesCriteria()
    {
    }

    public FeaturesCriteria(
        string requirements,
        CompliancyLevel compliance)
    {
        Requirements = requirements;
        Compliance = compliance;
    }

    public int Id { get; set; }

    public int NonPriceElementsId { get; set; }

    public string Requirements { get; set; }

    public CompliancyLevel Compliance { get; set; }
}
