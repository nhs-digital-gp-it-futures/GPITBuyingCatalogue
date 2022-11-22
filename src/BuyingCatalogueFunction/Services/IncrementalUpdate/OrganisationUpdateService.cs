using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Adapters;
using BuyingCatalogueFunction.Models.Ods;
using BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace BuyingCatalogueFunction.Services.IncrementalUpdate
{
    public class OrganisationUpdateService : IOrganisationUpdateService
    {
        private readonly BuyingCatalogueDbContext _dbContext;
        private readonly IAdapter<Organisation, OdsOrganisation> _organisationAdapter;
        private readonly IAdapter<Organisation, IEnumerable<OrganisationRelationship>> _relationshipsAdapter;
        private readonly IAdapter<Organisation, IEnumerable<OrganisationRole>> _rolesAdapter;

        public OrganisationUpdateService(
            BuyingCatalogueDbContext dbContext,
            IAdapter<Organisation, OdsOrganisation> organisationAdapter,
            IAdapter<Organisation, IEnumerable<OrganisationRelationship>> relationshipsAdapter,
            IAdapter<Organisation, IEnumerable<OrganisationRole>> rolesAdapter)
        {
            _dbContext = dbContext;
            _organisationAdapter = organisationAdapter;
            _relationshipsAdapter = relationshipsAdapter;
            _rolesAdapter = rolesAdapter;
        }

        public async Task Upsert(Organisation organisation)
        {
            var orgId = organisation.OrgId.extension;
            var existing = await _dbContext.OdsOrganisations.FirstOrDefaultAsync(x => x.Id == orgId);

            if (existing != null)
            {
                var relationships = await _dbContext.OrganisationRelationships
                    .Where(x => x.OwnerOrganisationId == orgId)
                    .ToListAsync();

                var roles = await _dbContext.OrganisationRoles
                    .Where(x => x.OrganisationId == orgId)
                    .ToListAsync();

                _dbContext.OrganisationRelationships.RemoveRange(relationships);
                _dbContext.OrganisationRoles.RemoveRange(roles);
                _dbContext.OdsOrganisations.Remove(existing);

                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.OdsOrganisations.AddAsync(_organisationAdapter.Process(organisation));
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddMissingOrganisations(IEnumerable<string> organisationIds)
        {
            var existingIds = await _dbContext.OdsOrganisations
                .Where(x => organisationIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            var idsToAdd = organisationIds.Except(existingIds);

            await _dbContext.OdsOrganisations.AddRangeAsync(idsToAdd.Select(x => new OdsOrganisation
            {
                Id = x,
                Name = "Pending",
            }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddRelationships(Organisation organisation)
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
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddRoles(Organisation organisation)
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
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddRelationshipTypes(IEnumerable<string> relationshipIds)
        {
            var existingIds = await _dbContext.OrganisationRelationshipTypes
                .Where(x => relationshipIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            var idsToAdd = relationshipIds.Except(existingIds);

            await _dbContext.OrganisationRelationshipTypes.AddRangeAsync(idsToAdd.Select(x => new RelationshipType
            {
                Id = x,
                Description = "Pending",
            }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddRoleTypes(IEnumerable<string> roleIds)
        {
            var existingIds = await _dbContext.OrganisationRoleTypes
                .Where(x => roleIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            var idsToAdd = roleIds.Except(existingIds);

            await _dbContext.OrganisationRoleTypes.AddRangeAsync(idsToAdd.Select(x => new RoleType
            {
                Id = x,
                Description = "Pending",
            }));

            await _dbContext.SaveChangesAsync();
        }
    }
}
