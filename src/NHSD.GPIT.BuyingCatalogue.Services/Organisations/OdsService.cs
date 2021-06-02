using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class OdsService : IOdsService
    {
        private const int DefaultCacheDuration = 60;

        private readonly ILogWrapper<OdsService> logger;
        private readonly OdsSettings settings;
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions memoryCacheOptions;

        public OdsService(ILogWrapper<OdsService> logger, OdsSettings settings, IMemoryCache memoryCache)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            memoryCacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(DefaultCacheDuration));
        }

        public async Task<OdsOrganisation> GetOrganisationByOdsCode(string odsCode)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
                throw new ArgumentException(odsCode);

            var key = $"ODS-{odsCode}";

            if (memoryCache.TryGetValue(key, out OdsOrganisation odsOrganisation))
                return odsOrganisation;

            var response = await settings.ApiBaseUrl
                .AppendPathSegment("organisations")
                .AppendPathSegment(odsCode)
                .AllowHttpStatus("3xx,4xx")
                .GetJsonAsync<OdsResponse>();

            var odsResponseOrganisation = response?.Organisation;

            odsOrganisation = odsResponseOrganisation is null ? null : new OdsOrganisation
            {
                OrganisationName = odsResponseOrganisation.Name,
                OdsCode = odsCode,
                PrimaryRoleId = GetPrimaryRoleId(odsResponseOrganisation),
                Address = OdsResponseAddressToAddress(odsResponseOrganisation.GeoLoc.Location),
                IsActive = IsActive(odsResponseOrganisation),
                IsBuyerOrganisation = IsBuyerOrganisation(odsResponseOrganisation),
            };

            if (odsOrganisation is null)
                return null;

            // TODO
            // if (!(odsOrganisation.IsActive && odsOrganisation.IsBuyerOrganisation))
            //    return new StatusCodeResult(StatusCodes.Status406NotAcceptable);
            memoryCache.Set(key, odsOrganisation, memoryCacheOptions);

            return odsOrganisation;
        }

        private static string GetPrimaryRoleId(OdsResponseOrganisation organisation)
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
            return settings.BuyerOrganisationRoleIds.Contains(GetPrimaryRoleId(organisation));
        }
    }
}
