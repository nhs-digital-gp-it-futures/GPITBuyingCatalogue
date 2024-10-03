using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;

public class DataProcessingInformationModel(CatalogueItem item, CatalogueItemContentStatus contentStatus)
    : SolutionDisplayBaseModel(item, contentStatus)
{
    public override int Index => 11;

    public DataProcessingInformation Information { get; set; } = item.Solution.DataProcessingInformation;
}
