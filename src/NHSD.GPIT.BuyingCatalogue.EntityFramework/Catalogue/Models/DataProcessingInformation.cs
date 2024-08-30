using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class DataProcessingInformation
{
    public int Id { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public int? DataProcessingDetailsId { get; set; }

    public DataProcessingDetails Details { get; set; }

    public DataProcessingLocation Location { get; set; }

    public DataProtectionOfficer Officer { get; set; }

    public ICollection<DataProtectionSubProcessor> SubProcessors { get; set; } =
        new HashSet<DataProtectionSubProcessor>();
}
