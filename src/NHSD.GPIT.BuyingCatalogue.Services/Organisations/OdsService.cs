using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class OdsService : IOdsService
    {
        private readonly ILogWrapper<OdsService> _logger;
        private readonly OdsSettings _settings;
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _memoryCacheOptions;

        private const int DEFAULT_CACHE_DURATION = 60;

        public OdsService(ILogWrapper<OdsService> logger, OdsSettings settings, IMemoryCache memoryCache)            
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            _memoryCacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(DEFAULT_CACHE_DURATION));
        }

        public async Task<OdsOrganisation> GetOrganisationByOdsCode(string odsCode)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
                throw new ArgumentException(odsCode);

            var key = $"ODS-{odsCode}";

            OdsOrganisation odsOrganisation;

            if ( _memoryCache.TryGetValue(key, out odsOrganisation))
                return odsOrganisation;
            
            var response = await _settings.ApiBaseUrl
                .AppendPathSegment("organisations")
                .AppendPathSegment(odsCode)
                .AllowHttpStatus("3xx,4xx")
                .GetJsonAsync<OdsResponse>();

            var odsResponseOrganisation = response?.Organisation;

            odsOrganisation =  odsResponseOrganisation is null ? null : new OdsOrganisation
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
            //if (!(odsOrganisation.IsActive && odsOrganisation.IsBuyerOrganisation))
            //    return new StatusCodeResult(StatusCodes.Status406NotAcceptable);

            _memoryCache.Set(key, odsOrganisation, _memoryCacheOptions);

            return odsOrganisation;
        }

        private static string GetPrimaryRoleId(OdsResponseOrganisation organisation)
        {
            return organisation.Roles.Role.FirstOrDefault(r => r.primaryRole)?.id;
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
            return _settings.BuyerOrganisationRoleIds.Contains(GetPrimaryRoleId(organisation));
        }
    }
}
