using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public sealed class OdsService : IOdsService
    {
        private const int DefaultCacheDuration = 60;

        private readonly OdsSettings settings;
        private readonly ILogger<OdsService> logger;
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions memoryCacheOptions;
        private readonly IOrganisationsService organisationsService;

        public OdsService(
            OdsSettings settings,
            ILogger<OdsService> logger,
            IMemoryCache memoryCache,
            IOrganisationsService organisationsService)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));

            memoryCacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(DefaultCacheDuration));
        }

        public async Task<(OdsOrganisation Organisation, string Error)> GetOrganisationByOdsCode(string odsCode)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
                throw new ArgumentException(odsCode);

            var key = $"ODS-{odsCode}";

            if (memoryCache.TryGetValue(key, out OdsOrganisation odsOrganisation))
                return (odsOrganisation, null);

            var response = await settings.ApiBaseUrl
                .AppendPathSegment("organisations")
                .AppendPathSegment(odsCode)
                .AllowHttpStatus("3xx,4xx")
                .GetJsonAsync<OdsResponse>();

            var odsResponseOrganisation = response?.Organisation;

            odsOrganisation = odsResponseOrganisation is null ? null : new OdsOrganisation
            {
                OrganisationName = odsResponseOrganisation.Name,
                OdsCode = odsResponseOrganisation.OrgId.Extension,
                OrganisationRoleId = GetOrganisationRoleId(odsResponseOrganisation),
                Address = OdsResponseAddressToAddress(odsResponseOrganisation.GeoLoc.Location),
                IsActive = IsActive(odsResponseOrganisation),
                IsBuyerOrganisation = IsBuyerOrganisation(odsResponseOrganisation),
            };

            if (odsOrganisation is null)
                return (null, "Organisation not found");

            if (!(odsOrganisation.IsActive && odsOrganisation.IsBuyerOrganisation))
                return (null, "Not a buyer organisation");

            memoryCache.Set(key, odsOrganisation, memoryCacheOptions);

            return (odsOrganisation, null);
        }

        public async Task<IEnumerable<ServiceRecipient>> GetServiceRecipientsByParentInternalIdentifier(string internalIdentifier)
        {
            if (string.IsNullOrWhiteSpace(internalIdentifier))
                throw new ArgumentException(internalIdentifier);

            var key = $"ServiceRecipients-Identifier-{internalIdentifier}";

            if (memoryCache.TryGetValue(key, out IEnumerable<ServiceRecipient> cachedResults))
                return cachedResults;

            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalIdentifier);

            var retrievedAll = false;

            var costCentres = new List<ServiceRecipient>();
            int offset = 0;
            int searchLimit = settings.GetChildOrganisationSearchLimit;

            while (!retrievedAll)
            {
                var query = settings.ApiBaseUrl
                    .AppendPathSegment("organisations")
                    .SetQueryParam("RelTypeId", "RE4")
                    .SetQueryParam("TargetOrgId", organisation.ExternalIdentifier)
                    .SetQueryParam("RelStatus", "active")
                    .SetQueryParam("Limit", searchLimit)
                    .AllowHttpStatus("3xx,4xx");

                if (offset > 0)
                {
                    query.SetQueryParam("Offset", offset);
                }

                var serviceRecipientResponse = await query.GetJsonAsync<ServiceRecipientResponse>();

                if (serviceRecipientResponse.Organisations is null)
                {
                    break;
                }

                var centres =
                    serviceRecipientResponse.Organisations.Where(o => o.PrimaryRoleId == settings.GpPracticeRoleId)
                        .Select(
                            x => new ServiceRecipient
                            {
                                Name = x.Name, OrgId = x.OrgId, OrganisationRoleId = x.PrimaryRoleId, Status = x.Status,
                            });

                costCentres.AddRange(centres);

                retrievedAll = serviceRecipientResponse.Organisations.Count() != searchLimit;
                offset += searchLimit;
            }

            memoryCache.Set(key, costCentres, memoryCacheOptions);

            return costCentres;
        }

        public async Task UpdateOrganisationDetails(string odsCode)
        {
            var (organisation, errorMessage) = await GetOrganisationByOdsCode(odsCode);

            if (!string.IsNullOrWhiteSpace(errorMessage)
                || organisation == null)
            {
                logger.LogWarning("No spine entry found for ODS code {OdsCode}", odsCode);
                return;
            }

            await organisationsService.UpdateCcgOrganisation(organisation);
        }

        private static string GetOrganisationRoleId(OdsResponseOrganisation organisation)
        {
            return organisation.Roles.Role.FirstOrDefault(r => r.PrimaryRole)?.Id;
        }

        private static Address OdsResponseAddressToAddress(OdsResponseAddress odsAddress)
        {
            return new()
            {
                Line1 = odsAddress.AddrLn1,
                Line2 = odsAddress.AddrLn2,
                Line3 = odsAddress.AddrLn3,
                Line4 = odsAddress.AddrLn4,
                Town = odsAddress.Town,
                County = odsAddress.County,
                Postcode = odsAddress.PostCode,
                Country = odsAddress.Country,
            };
        }

        private static bool IsActive(OdsResponseOrganisation organisation)
        {
            return organisation.Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsBuyerOrganisation(OdsResponseOrganisation organisation)
        {
            return settings.BuyerOrganisationRoleIds.Contains(GetOrganisationRoleId(organisation));
        }

        internal sealed class ServiceRecipientResponse
        {
            public IEnumerable<ODSServiceRecipient> Organisations { get; set; }
        }

        internal sealed class ODSServiceRecipient
        {
            public string Name { get; set; }

            public string OrgId { get; set; }

            public string Status { get; set; }

            public string PrimaryRoleId { get; set; }
        }
    }
}
