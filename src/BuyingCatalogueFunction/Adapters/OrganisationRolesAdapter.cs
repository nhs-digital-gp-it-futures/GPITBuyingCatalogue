using System;
using System.Collections.Generic;
using System.Linq;
using BuyingCatalogueFunction.Models.Ods;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace BuyingCatalogueFunction.Adapters
{
    public class OrganisationRolesAdapter : IAdapter<Org, IEnumerable<OrganisationRole>>
    {
        public IEnumerable<OrganisationRole> Process(Org input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var roles = input.Roles?.Role ?? Enumerable.Empty<OrgRole>();

            return roles.Select(x => new OrganisationRole
            {
                Id = x.uniqueRoleId,
                RoleId = x.id,
                OrganisationId = input.OrgId.extension,
                IsPrimaryRole = x.primaryRole ?? false,
            });
        }
    }
}
