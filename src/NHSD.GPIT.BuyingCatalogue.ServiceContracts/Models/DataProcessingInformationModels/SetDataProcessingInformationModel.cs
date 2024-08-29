namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DataProcessingInformationModels;

public record SetDataProcessingInformationModel(string Subject, string Duration, string ProcessingNature, string PersonalDataTypes, string DataSubjectCategories, string ProcessingLocation, string AdditionalJurisdiction = null);
