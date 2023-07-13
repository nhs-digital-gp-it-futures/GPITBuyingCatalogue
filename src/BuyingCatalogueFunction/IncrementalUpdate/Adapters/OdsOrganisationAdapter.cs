using System;
using BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace BuyingCatalogueFunction.IncrementalUpdate.Adapters
{
    public class OdsOrganisationAdapter : IAdapter<Org, OdsOrganisation>
    {
        public OdsOrganisation Process(Org input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return new OdsOrganisation
            {
                Id = input.OrgId.extension,
                Name = input.Name,
                IsActive = string.Equals(input.Status, Org.Active, StringComparison.InvariantCultureIgnoreCase),
                AddressLine1 = input.GeoLoc.Location.AddrLn1,
                AddressLine2 = input.GeoLoc.Location.AddrLn2,
                AddressLine3 = input.GeoLoc.Location.AddrLn3,
                Town = input.GeoLoc.Location.Town,
                County = input.GeoLoc.Location.County,
                Postcode = input.GeoLoc.Location.PostCode,
                Country = input.GeoLoc.Location.Country,
            };
        }
    }
}
