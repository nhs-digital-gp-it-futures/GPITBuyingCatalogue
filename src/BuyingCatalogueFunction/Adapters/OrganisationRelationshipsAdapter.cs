using System;
using System.Collections.Generic;
using System.Linq;
using BuyingCatalogueFunction.Models.Ods;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace BuyingCatalogueFunction.Adapters
{
    public class OrganisationRelationshipsAdapter : IAdapter<Org, IEnumerable<OrganisationRelationship>>
    {
        public IEnumerable<OrganisationRelationship> Process(Org input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var relationships = input.Rels?.Rel ?? Enumerable.Empty<OrgRel>();

            return relationships.Select(x => new OrganisationRelationship
            {
                Id = x.uniqueRelId,
                RelationshipTypeId = x.id,
                OwnerOrganisationId = input.OrgId.extension,
                TargetOrganisationId = x.Target.OrgId.extension,
            });
        }
    }
}
