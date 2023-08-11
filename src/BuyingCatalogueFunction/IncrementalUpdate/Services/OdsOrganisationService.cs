using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.IncrementalUpdate.Adapters;
using BuyingCatalogueFunction.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace BuyingCatalogueFunction.IncrementalUpdate.Services
{
    public class OdsOrganisationService : IOdsOrganisationService
    {
        private readonly BuyingCatalogueDbContext _dbContext;
        private readonly IAdapter<Org, OdsOrganisation> _organisationAdapter;
        private readonly IAdapter<Org, IEnumerable<OrganisationRelationship>> _relationshipsAdapter;
        private readonly IAdapter<Org, IEnumerable<OrganisationRole>> _rolesAdapter;

        public OdsOrganisationService(
            BuyingCatalogueDbContext dbContext,
            IAdapter<Org, OdsOrganisation> organisationAdapter,
            IAdapter<Org, IEnumerable<OrganisationRelationship>> relationshipsAdapter,
            IAdapter<Org, IEnumerable<OrganisationRole>> rolesAdapter)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _organisationAdapter = organisationAdapter ?? throw new ArgumentNullException(nameof(organisationAdapter));
            _relationshipsAdapter = relationshipsAdapter ?? throw new ArgumentNullException(nameof(relationshipsAdapter));
            _rolesAdapter = rolesAdapter ?? throw new ArgumentNullException(nameof(rolesAdapter));
        }

        public async Task AddRelationshipTypes(List<Relationship> relationships)
        {
            if (relationships == null)
                throw new ArgumentNullException(nameof(relationships));

            var relationshipIds = relationships.Select(x => x.id);

            var existingIds = await _dbContext.OrganisationRelationshipTypes
                .Where(x => relationshipIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            var newRelationshipTypes = relationshipIds.Except(existingIds)
                .Select(x => relationships.First(r => r.id == x))
                .Select(x => new RelationshipType
                {
                    Id = x.id,
                    Description = x.displayName,
                });

            await _dbContext.OrganisationRelationshipTypes.AddRangeAsync(newRelationshipTypes);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddRoleTypes(List<Role> roles)
        {
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            var roleIds = roles.Select(x => x.id);

            var existingIds = await _dbContext.OrganisationRoleTypes
                .Where(x => roleIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            var newRoleTypes = roleIds.Except(existingIds)
                .Select(x => roles.First(r => r.id == x))
                .Select(x => new RoleType
                {
                    Id = x.id,
                    Description = x.displayName,
                });

            await _dbContext.OrganisationRoleTypes.AddRangeAsync(newRoleTypes);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddOrganisationRelationships(List<Org> organisations)
        {
            if (organisations == null)
                throw new ArgumentNullException(nameof(organisations));

            foreach (var organisation in organisations)
            {
                var existing = await _dbContext.OrganisationRelationships
                    .Where(x => x.OwnerOrganisationId == organisation.OrgId.extension)
                    .ToListAsync();

                if (existing.Any())
                {
                    _dbContext.OrganisationRelationships.RemoveRange(existing);

                    await _dbContext.SaveChangesAsync();
                }

                await _dbContext.OrganisationRelationships.AddRangeAsync(_relationshipsAdapter.Process(organisation));
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddOrganisationRoles(List<Org> organisations)
        {
            if (organisations == null)
                throw new ArgumentNullException(nameof(organisations));

            foreach (var organisation in organisations)
            {
                var existing = await _dbContext.OrganisationRoles
                    .Where(x => x.OrganisationId == organisation.OrgId.extension)
                    .ToListAsync();

                if (existing.Any())
                {
                    _dbContext.OrganisationRoles.RemoveRange(existing);

                    await _dbContext.SaveChangesAsync();
                }

                await _dbContext.OrganisationRoles.AddRangeAsync(_rolesAdapter.Process(organisation));
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpsertOrganisations(List<Org> organisations)
        {
            if (organisations == null)
                throw new ArgumentNullException(nameof(organisations));

            var odsOrganisations = organisations.Select(_organisationAdapter.Process);

            foreach (var odsOrganisation in odsOrganisations)
            {
                var existing = await _dbContext.OdsOrganisations.FirstOrDefaultAsync(x => x.Id == odsOrganisation.Id);

                if (existing == null)
                    await _dbContext.OdsOrganisations.AddAsync(odsOrganisation);
                else
                    existing.UpdateFrom(odsOrganisation);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
