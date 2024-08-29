using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DataProcessingInformationModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

public interface IDataProcessingInformationService
{
    Task<Solution> GetSolutionWithDataProcessingInformation(CatalogueItemId solutionId);

    Task SetDataProcessingInformation(
        CatalogueItemId solutionId,
        SetDataProcessingInformationModel model);
}
