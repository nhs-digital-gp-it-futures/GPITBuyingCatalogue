using Flurl;
using Flurl.Http;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class OdsService : IOdsService
    {
        private readonly ILogWrapper<OdsService> _logger;
        private readonly OdsSettings _settings;

        public OdsService(ILogWrapper<OdsService> logger, OdsSettings settings)            
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<OdsOrganisation> GetOrganisationByOdsCodeAsync(string odsCode)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
                throw new ArgumentException(odsCode);

            var odsOrganisation = await GetOrganisationByOdsCodeAsync2(odsCode);

            if (odsOrganisation is null)
                return null;

            // TODO
            //if (!(odsOrganisation.IsActive && odsOrganisation.IsBuyerOrganisation))
            //    return new StatusCodeResult(StatusCodes.Status406NotAcceptable);

            var addressModel = odsOrganisation.Address is null ? null : new Address
            {
                Line1 = odsOrganisation.Address.Line1,
                Line2 = odsOrganisation.Address.Line2,
                Line3 = odsOrganisation.Address.Line3,
                Line4 = odsOrganisation.Address.Line4,
                Town = odsOrganisation.Address.Town,
                County = odsOrganisation.Address.County,
                Postcode = odsOrganisation.Address.Postcode,
                Country = odsOrganisation.Address.Country,
            };

            return new OdsOrganisation
            {
                OdsCode = odsOrganisation.OdsCode,
                OrganisationName = odsOrganisation.OrganisationName,
                PrimaryRoleId = odsOrganisation.PrimaryRoleId,
                Address = addressModel,
            };
        }

        private async Task<OdsOrganisation> GetOrganisationByOdsCodeAsync2(string odsCode)
        {
            var response = await _settings.ApiBaseUrl
                .AppendPathSegment("organisations")
                .AppendPathSegment(odsCode)
                .AllowHttpStatus("3xx,4xx")
                .GetJsonAsync<OdsResponse>();

            var odsOrganisation = response?.Organisation;

            return odsOrganisation is null ? null : new OdsOrganisation
            {
                OrganisationName = odsOrganisation.Name,
                OdsCode = odsCode,
                PrimaryRoleId = GetPrimaryRoleId(odsOrganisation),
                Address = OdsResponseAddressToAddress(odsOrganisation.GeoLoc.Location),
                IsActive = IsActive(odsOrganisation),
                IsBuyerOrganisation = IsBuyerOrganisation(odsOrganisation),
            };
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
