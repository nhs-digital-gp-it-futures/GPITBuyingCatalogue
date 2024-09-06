using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

public class AddEditDataProcessingInformationModel : NavBaseModel
{
    public AddEditDataProcessingInformationModel()
    {
    }

    public AddEditDataProcessingInformationModel(Solution solution)
    {
        var solutionDataProcessing = solution.DataProcessingInformation;

        SolutionName = solution.CatalogueItem.Name;
        Subject = solutionDataProcessing?.Details?.Subject;
        Duration = solutionDataProcessing?.Details?.Duration;
        ProcessingNature = solutionDataProcessing?.Details?.ProcessingNature;
        PersonalDataTypes = solutionDataProcessing?.Details?.PersonalDataTypes;
        DataSubjectCategories = solutionDataProcessing?.Details?.DataSubjectCategories;
        ProcessingLocation = solutionDataProcessing?.Location?.ProcessingLocation;
        AdditionalJurisdiction = solutionDataProcessing?.Location?.AdditionalJurisdiction;
    }

    public string SolutionName { get; set; }

    [StringLength(2000)]
    public string Subject { get; set; }

    [StringLength(2000)]
    public string Duration { get; set; }

    [StringLength(2000)]
    public string ProcessingNature { get; set; }

    [StringLength(2000)]
    public string PersonalDataTypes { get; set; }

    [StringLength(2000)]
    public string DataSubjectCategories { get; set; }

    [StringLength(2000)]
    public string ProcessingLocation { get; set; }

    [StringLength(2000)]
    public string AdditionalJurisdiction { get; set; }
}
