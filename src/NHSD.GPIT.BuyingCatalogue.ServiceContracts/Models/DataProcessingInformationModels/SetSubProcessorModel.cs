namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DataProcessingInformationModels;

public record SetSubProcessorModel(
    string OrganisationName,
    string Subject,
    string Duration,
    string ProcessingNature,
    string PersonalDataTypes,
    string DataSubjectCategories,
    string PostProcessingPlan,
    int? SubProcessorId = null);
