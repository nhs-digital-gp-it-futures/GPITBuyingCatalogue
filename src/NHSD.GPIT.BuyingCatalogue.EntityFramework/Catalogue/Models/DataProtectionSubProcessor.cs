namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class DataProtectionSubProcessor
{
    public int Id { get; set; }

    public int DataProcessingInfoId { get; set; }

    public int DataProcessingDetailsId { get; set; }

    public string OrganisationName { get; set; }

    public string PostProcessingPlan { get; set; }

    public DataProcessingDetails Details { get; set; }
}
