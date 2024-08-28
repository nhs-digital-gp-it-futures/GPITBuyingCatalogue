using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ServiceLevelAgreements;
using NHSD.GPIT.BuyingCatalogue.Services.Properties;

namespace NHSD.GPIT.BuyingCatalogue.Services.ServiceLevelAgreements
{
    public sealed class ServiceLevelAgreementsService : IServiceLevelAgreementsService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ServiceLevelAgreementsService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddServiceLevelAgreement(AddSlaModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var solution = await dbContext.Solutions.FirstAsync(s => s.CatalogueItemId == model.Solution.Id);
            solution.ServiceLevelAgreement = new EntityFramework.Catalogue.Models.ServiceLevelAgreements
            {
                SlaType = model.SlaLevel,
            };

            if (solution.ServiceLevelAgreement.SlaType == SlaType.Type1)
                ApplyType1ServiceLevelAgreement(solution.ServiceLevelAgreement);

            await dbContext.SaveChangesAsync();
        }

        public async Task<EntityFramework.Catalogue.Models.ServiceLevelAgreements> GetServiceLevelAgreementForSolution(CatalogueItemId solutionId)
        {
            return await dbContext.ServiceLevelAgreements
                .Include(s => s.Contacts)
                .Include(s => s.ServiceHours)
                .Include(s => s.ServiceLevels)
                .FirstOrDefaultAsync(s => s.SolutionId == solutionId);
        }

        public async Task UpdateServiceLevelTypeAsync(CatalogueItem solution, SlaType slaLevel)
        {
            var sla = await dbContext.ServiceLevelAgreements.Include(x => x.ServiceHours)
                .Include(x => x.ServiceLevels)
                .FirstAsync(s => s.SolutionId == solution.Id);

            if (sla.SlaType != slaLevel)
            {
                sla.ServiceHours.Clear();
                sla.ServiceLevels.Clear();

                if (slaLevel == SlaType.Type1)
                    ApplyType1ServiceLevelAgreement(sla);
            }

            sla.SlaType = slaLevel;

            await dbContext.SaveChangesAsync();
        }

        public async Task<ServiceAvailabilityTimes> GetServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId)
            => await dbContext.ServiceAvailabilityTimes.FirstOrDefaultAsync(s => s.SolutionId == solutionId && s.Id == serviceAvailabilityTimesId);

        public async Task SaveServiceAvailabilityTimes(CatalogueItem solution, ServiceAvailabilityTimesModel model)
        {
            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var serviceAvailabilityTimes = new ServiceAvailabilityTimes
            {
                IncludedDays = model.ApplicableDays,
                IncludesBankHolidays = model.IncludesBankHolidays,
                AdditionalInformation = model.AdditionalInformation,
                Category = model.SupportType,
                TimeFrom = model.From,
                TimeUntil = model.Until,
                SolutionId = solution.Id,
            };

            dbContext.ServiceAvailabilityTimes.Add(serviceAvailabilityTimes);

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateServiceAvailabilityTimes(CatalogueItem solution, int serviceAvailabilityTimesId, ServiceAvailabilityTimesModel model)
        {
            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var serviceAvailabilityTimes = await GetServiceAvailabilityTimes(solution.Id, serviceAvailabilityTimesId);

            serviceAvailabilityTimes.IncludedDays = model.ApplicableDays;
            serviceAvailabilityTimes.IncludesBankHolidays = model.IncludesBankHolidays;
            serviceAvailabilityTimes.AdditionalInformation = model.AdditionalInformation;
            serviceAvailabilityTimes.Category = model.SupportType;
            serviceAvailabilityTimes.TimeFrom = model.From;
            serviceAvailabilityTimes.TimeUntil = model.Until;
            serviceAvailabilityTimes.LastUpdatedBy = model.UserId;
            serviceAvailabilityTimes.LastUpdated = DateTime.UtcNow;

            dbContext.ServiceAvailabilityTimes.Update(serviceAvailabilityTimes);

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteServiceAvailabilityTimes(CatalogueItemId solutionId, int serviceAvailabilityTimesId)
        {
            var serviceAvailabilityTimes = await GetServiceAvailabilityTimes(solutionId, serviceAvailabilityTimesId);

            dbContext.ServiceAvailabilityTimes.Remove(serviceAvailabilityTimes);

            await dbContext.SaveChangesAsync();
        }

        public async Task<int> GetCountOfServiceAvailabilityTimes(CatalogueItemId solutionId, params int[] idsToExclude)
            => await dbContext.ServiceAvailabilityTimes.Where(s => s.SolutionId == solutionId && !idsToExclude.Contains(s.Id)).CountAsync();

        public async Task AddSLAContact(CatalogueItem solution, EditSLAContactModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            var slaContact = new SlaContact
            {
                SolutionId = solution.Id,
                Channel = model.Channel,
                ContactInformation = model.ContactInformation,
                ApplicableDays = model.ApplicableDays,
                TimeFrom = model.TimeFrom,
                TimeUntil = model.TimeUntil,
                LastUpdated = DateTime.UtcNow,
                LastUpdatedBy = model.UserId,
            };

            dbContext.SlaContacts.Add(slaContact);

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteSlaContact(int slaContactId)
        {
            var contact = await dbContext.SlaContacts.FirstAsync(slac => slac.Id == slaContactId);

            dbContext.SlaContacts.Remove(contact);

            await dbContext.SaveChangesAsync();
        }

        public async Task EditSlaContact(EditSLAContactModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var contact = await dbContext.SlaContacts.FirstAsync(slac => slac.Id == model.Id);

            contact.Channel = model.Channel;
            contact.ContactInformation = model.ContactInformation;
            contact.ApplicableDays = model.ApplicableDays;
            contact.TimeFrom = model.TimeFrom;
            contact.TimeUntil = model.TimeUntil;

            await dbContext.SaveChangesAsync();
        }

        public async Task AddServiceLevel(CatalogueItemId solutionId, EditServiceLevelModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var serviceLevel = new SlaServiceLevel
            {
                SolutionId = solutionId,
                TypeOfService = model.ServiceType,
                ServiceLevel = model.ServiceLevel,
                HowMeasured = model.HowMeasured,
                ServiceCredits = model.CreditsApplied,
            };

            dbContext.SlaServiceLevels.Add(serviceLevel);

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateServiceLevel(CatalogueItemId solutionId, int serviceLevelId, EditServiceLevelModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var serviceLevel = await GetServiceLevel(solutionId, serviceLevelId);
            serviceLevel.TypeOfService = model.ServiceType;
            serviceLevel.ServiceLevel = model.ServiceLevel;
            serviceLevel.HowMeasured = model.HowMeasured;
            serviceLevel.ServiceCredits = model.CreditsApplied;

            dbContext.SlaServiceLevels.Update(serviceLevel);

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteServiceLevel(CatalogueItemId solutionId, int serviceLevelId)
        {
            var serviceLevel = await GetServiceLevel(solutionId, serviceLevelId);

            dbContext.SlaServiceLevels.Remove(serviceLevel);

            await dbContext.SaveChangesAsync();
        }

        private static void ApplyType1ServiceLevelAgreement(EntityFramework.Catalogue.Models.ServiceLevelAgreements serviceLevelAgreement)
        {
            serviceLevelAgreement.ServiceHours.AddRange(
                JsonDeserializer.Deserialize<ICollection<ServiceAvailabilityTimes>>(Resources.Type1DefaultServiceHours.GetString()));

            serviceLevelAgreement.ServiceLevels.AddRange(
                JsonDeserializer.Deserialize<ICollection<SlaServiceLevel>>(Resources.Type1DefaultServiceLevels.GetString()));
        }

        private async Task<SlaServiceLevel> GetServiceLevel(CatalogueItemId solutionId, int serviceLevelId)
            => await dbContext.SlaServiceLevels.FirstOrDefaultAsync(s => s.SolutionId == solutionId && s.Id == serviceLevelId);
    }
}
