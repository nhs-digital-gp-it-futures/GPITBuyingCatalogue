using System;
using Bogus;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData
{
    internal static class GenerateCatalogueSolution
    {
        internal static CatalogueItem Generate(CatalogueItemId id)
        {
            var catalogueItem = new Faker<CatalogueItem>()
                .RuleFor(ci => ci.Id, _ => id)
                .RuleFor(ci => ci.SupplierId, _ => id.SupplierId)
                .RuleFor(ci => ci.CatalogueItemType, _ => CatalogueItemType.Solution)
                .RuleFor(ci => ci.Created, _ => DateTime.UtcNow)
                .RuleFor(ci => ci.Name, f => $"{f.Name.JobTitle()}(ノಠ益ಠ)ノ彡┻━┻{id.ItemId}")
                .RuleFor(ci => ci.PublishedStatus, _ => PublicationStatus.Published)
                .Generate();

            catalogueItem.Solution = new Faker<Solution>()
                .RuleFor(s => s.CatalogueItemId, _ => id)
                .RuleFor(s => s.Summary, f => string.Join(" ", f.Lorem.Words(10)))
                .RuleFor(s => s.FullDescription, f => string.Join(" ", f.Lorem.Words(100)))
                .Generate();

            return catalogueItem;
        }
    }
}
