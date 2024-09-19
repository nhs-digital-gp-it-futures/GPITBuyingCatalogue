namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class DataProtectionOfficer
{
    public int Id { get; set; }

    public int DataProcessingInfoId { get; set; }

    public string Name { get; set; }

    public string EmailAddress { get; set; }

    public string PhoneNumber { get; set; }
}
