using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;

public interface ISolutionPublicationStatusService
{
    Task SetPublicationStatus(CatalogueItemId catalogueItemId, PublicationStatus publicationStatus);
}
