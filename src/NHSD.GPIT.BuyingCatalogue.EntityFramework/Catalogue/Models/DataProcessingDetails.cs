namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public class DataProcessingDetails
{
    public DataProcessingDetails()
    {
    }

    public DataProcessingDetails(
        string subject,
        string duration,
        string processingNature,
        string personalDataTypes,
        string dataSubjectCategories)
    {
        Subject = subject;
        Duration = duration;
        ProcessingNature = processingNature;
        PersonalDataTypes = personalDataTypes;
        DataSubjectCategories = dataSubjectCategories;
    }

    public int Id { get; set; }

    public string Subject { get; set; }

    public string Duration { get; set; }

    public string ProcessingNature { get; set; }

    public string PersonalDataTypes { get; set; }

    public string DataSubjectCategories { get; set; }
}
