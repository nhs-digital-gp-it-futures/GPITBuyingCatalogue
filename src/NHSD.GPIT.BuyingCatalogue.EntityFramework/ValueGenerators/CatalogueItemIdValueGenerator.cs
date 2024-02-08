using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.ValueGenerators
{
    public sealed class CatalogueItemIdValueGenerator : ValueGenerator<CatalogueItemId>
    {
        public override bool GeneratesTemporaryValues => false;

        public override CatalogueItemId Next(EntityEntry entry)
            => NextAsync(entry).AsTask().GetAwaiter().GetResult();

        [ExcludeFromCodeCoverage(Justification = "Not all code paths can be tested due to a reliance on internal Microsoft APIs")]
        public override async ValueTask<CatalogueItemId> NextAsync(EntityEntry entry, CancellationToken cancellationToken = default)
        {
            if (entry.Entity is not CatalogueItem catalogueItem)
                throw new ArgumentException($"Entity must be of type {nameof(CatalogueItem)}", nameof(entry));

            if (catalogueItem.Id != default)
                return catalogueItem.Id;

            var baseQuery = entry.Context.Set<CatalogueItem>()
                .Where(i => i.CatalogueItemType == catalogueItem.CatalogueItemType && i.SupplierId == catalogueItem.SupplierId);

            if (catalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService)
                baseQuery = baseQuery.Where(i => i.AdditionalService.SolutionId == catalogueItem.AdditionalService.SolutionId);

            var latestCatalogueItem = await baseQuery.OrderByDescending(i => i.Id).FirstOrDefaultAsync(cancellationToken);

            var incrementedCatalogueItemId = catalogueItem.CatalogueItemType switch
            {
                CatalogueItemType.AdditionalService => (latestCatalogueItem?.Id ?? new CatalogueItemId(catalogueItem.SupplierId, $"{catalogueItem.AdditionalService.SolutionId.ItemId}A000")).NextAdditionalServiceId(),
                CatalogueItemType.AssociatedService => (latestCatalogueItem?.Id ?? new CatalogueItemId(catalogueItem.SupplierId, "S-000")).NextAssociatedServiceId(),
                CatalogueItemType.Solution or _ => (latestCatalogueItem?.Id ?? new CatalogueItemId(catalogueItem.SupplierId, "000")).NextSolutionId(),
            };

            return incrementedCatalogueItemId;
        }
    }
}
