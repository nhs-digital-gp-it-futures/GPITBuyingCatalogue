using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

public class StandardComplianceModel
{
    public StandardComplianceModel()
    {
    }

    public StandardComplianceModel(Standard standard, StandardCompliance compliance)
    {
        Id = standard.Id;
        Name = standard.Name;
        Description = standard.Description;
        Url = standard.Url;
        Type = standard.StandardType;
        Compliance = compliance;
    }

    public string Id { get; }

    public string Name { get; }

    public string Description { get; }

    public string Url { get; }

    public StandardType Type { get; }

    public StandardCompliance Compliance { get; }
}
