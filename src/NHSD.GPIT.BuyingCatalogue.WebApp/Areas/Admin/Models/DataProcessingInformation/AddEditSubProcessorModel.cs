using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;

public class AddEditSubProcessorModel : NavBaseModel
{
    public AddEditSubProcessorModel()
    {
    }

    public AddEditSubProcessorModel(
        Solution solution)
    {
        SolutionId = solution.CatalogueItemId;
        SolutionName = solution.CatalogueItem.Name;
        PublicationStatus = solution.CatalogueItem.PublishedStatus;
    }

    public AddEditSubProcessorModel(
        Solution solution,
        DataProtectionSubProcessor subProcessor)
        : this(solution)
    {
        NumberOfSubProcessors = (solution.DataProcessingInformation?.SubProcessors?.Count).GetValueOrDefault();

        SubProcessorId = subProcessor?.Id;

        OrganisationName = subProcessor?.OrganisationName;
        PostProcessingPlan = subProcessor?.PostProcessingPlan;

        Subject = subProcessor?.Details?.Subject;
        Duration = subProcessor?.Details?.Duration;
        ProcessingNature = subProcessor?.Details?.ProcessingNature;
        PersonalDataTypes = subProcessor?.Details?.PersonalDataTypes;
        DataSubjectCategories = subProcessor?.Details?.DataSubjectCategories;
    }

    public CatalogueItemId SolutionId { get; set; }

    public string SolutionName { get; set; }

    public int NumberOfSubProcessors { get; set; }

    public PublicationStatus PublicationStatus { get; set; }

    public int? SubProcessorId { get; set; }

    public bool CanDelete => SubProcessorId is not null
        && ((NumberOfSubProcessors > 1
                && PublicationStatus is PublicationStatus.Published)
            || PublicationStatus is not PublicationStatus.Published);

    [StringLength(200)]
    public string OrganisationName { get; set; }

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
    public string PostProcessingPlan { get; set; }
}
