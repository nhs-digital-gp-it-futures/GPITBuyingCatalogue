using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DataProcessingInformationModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions;

public class DataProcessingInformationService(BuyingCatalogueDbContext dbContext) : IDataProcessingInformationService
{
    private readonly BuyingCatalogueDbContext dbContext =
        dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<Solution> GetSolutionWithDataProcessingInformation(CatalogueItemId solutionId) => await dbContext
        .Solutions
        .Include(x => x.CatalogueItem)
        .Include(x => x.DataProcessingInformation)
        .Include(x => x.DataProcessingInformation.Details)
        .Include(x => x.DataProcessingInformation.Location)
        .Include(x => x.DataProcessingInformation.Officer)
        .Include(x => x.DataProcessingInformation.SubProcessors)
        .ThenInclude(x => x.Details)
        .AsNoTracking()
        .AsSplitQuery()
        .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionId);

    public async Task SetDataProcessingInformation(
        CatalogueItemId solutionId,
        SetDataProcessingInformationModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var solution = await dbContext
            .Solutions
            .Include(x => x.DataProcessingInformation)
            .Include(x => x.DataProcessingInformation.Details)
            .Include(x => x.DataProcessingInformation.Location)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionId);

        var dataProcessingInformation = solution.DataProcessingInformation ??= new DataProcessingInformation();
        var dataProcessingDetails = dataProcessingInformation.Details ??= new DataProcessingDetails();
        var dataProcessingLocation = dataProcessingInformation.Location ??= new DataProcessingLocation();

        dataProcessingDetails.Subject = model.Subject;
        dataProcessingDetails.Duration = model.Duration;
        dataProcessingDetails.ProcessingNature = model.ProcessingNature;
        dataProcessingDetails.PersonalDataTypes = model.PersonalDataTypes;
        dataProcessingDetails.DataSubjectCategories = model.DataSubjectCategories;
        dataProcessingLocation.ProcessingLocation = model.ProcessingLocation;
        dataProcessingLocation.AdditionalJurisdiction = model.AdditionalJurisdiction;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetDataProtectionOfficer(CatalogueItemId solutionId, SetDataProtectionOfficerModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var solution = await dbContext
            .Solutions
            .Include(x => x.DataProcessingInformation)
            .Include(x => x.DataProcessingInformation.Officer)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionId);

        var dataProcessingInformation = solution.DataProcessingInformation ??= new DataProcessingInformation();
        var dataProtectionOfficer = dataProcessingInformation.Officer ??= new DataProtectionOfficer();

        dataProtectionOfficer.Name = model.Name;
        dataProtectionOfficer.EmailAddress = model.EmailAddress;
        dataProtectionOfficer.PhoneNumber = model.PhoneNumber;

        await dbContext.SaveChangesAsync();
    }

    public async Task AddSubProcessor(CatalogueItemId solutionId, SetSubProcessorModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var solution = await dbContext
            .Solutions
            .Include(x => x.DataProcessingInformation)
            .Include(x => x.DataProcessingInformation.SubProcessors)
            .ThenInclude(x => x.Details)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionId);

        var dataProcessingInformation = solution.DataProcessingInformation ??= new DataProcessingInformation();

        var subProcessor = new DataProtectionSubProcessor
        {
            OrganisationName = model.OrganisationName,
            PostProcessingPlan = model.PostProcessingPlan,
            Details = new DataProcessingDetails
            {
                Subject = model.Subject,
                Duration = model.Duration,
                ProcessingNature = model.ProcessingNature,
                PersonalDataTypes = model.PersonalDataTypes,
                DataSubjectCategories = model.DataSubjectCategories,
            },
        };

        dataProcessingInformation.SubProcessors.Add(subProcessor);

        await dbContext.SaveChangesAsync();
    }

    public async Task EditSubProcessor(CatalogueItemId solutionId, SetSubProcessorModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!model.SubProcessorId.HasValue) throw new ArgumentException("Invalid Sub-processor ID");

        var solution = await dbContext
            .Solutions
            .Include(x => x.DataProcessingInformation)
            .Include(x => x.DataProcessingInformation.SubProcessors)
            .ThenInclude(x => x.Details)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionId);

        var dataProcessingInformation = solution.DataProcessingInformation;

        var subProcessor = dataProcessingInformation?.SubProcessors.FirstOrDefault(x => x.Id == model.SubProcessorId);
        if (subProcessor is null) throw new InvalidOperationException("Invalid Sub-processor");

        var subProcessorDetails = subProcessor.Details ??= new DataProcessingDetails();

        subProcessorDetails.Subject = model.Subject;
        subProcessorDetails.Duration = model.Duration;
        subProcessorDetails.ProcessingNature = model.ProcessingNature;
        subProcessorDetails.PersonalDataTypes = model.PersonalDataTypes;
        subProcessorDetails.DataSubjectCategories = model.DataSubjectCategories;
        subProcessor.OrganisationName = model.OrganisationName;
        subProcessor.PostProcessingPlan = model.PostProcessingPlan;

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteSubProcessor(CatalogueItemId solutionId, int subProcessorId)
    {
        var solution = await dbContext
            .Solutions
            .Include(x => x.DataProcessingInformation)
            .Include(x => x.DataProcessingInformation.SubProcessors)
            .ThenInclude(x => x.Details)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionId);

        var dataProcessingInformation = solution.DataProcessingInformation;

        var subProcessor = dataProcessingInformation?.SubProcessors.FirstOrDefault(x => x.Id == subProcessorId);
        if (subProcessor is null) return;

        dataProcessingInformation.SubProcessors.Remove(subProcessor);

        await dbContext.SaveChangesAsync();
    }
}
