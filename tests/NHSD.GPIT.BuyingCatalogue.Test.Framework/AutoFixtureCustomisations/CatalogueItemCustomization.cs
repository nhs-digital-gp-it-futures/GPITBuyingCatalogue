using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CatalogueItemCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CatalogueItem> composer) => composer
                .FromFactory(new CatalogueItemSpecimenBuilder())
                .Without(i => i.AdditionalService)
                .Without(i => i.AssociatedService)
                .Without(i => i.CatalogueItemCapabilities)
                .Without(i => i.CatalogueItemContacts)
                .Without(i => i.CatalogueItemEpics)
                .Without(i => i.CatalogueItemType)
                .Without(i => i.CataloguePrices)
                .Without(i => i.Solution)
                .Without(i => i.Supplier)
                .Without(i => i.SupplierId)
                .Without(i => i.SupplierServiceAssociations)
                .Without(i => i.PublishedStatus);

            fixture.Customize<CatalogueItem>(ComposerTransformation);
        }

        private sealed class CatalogueItemSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(CatalogueItem)))
                    return new NoSpecimen();

                var id = context.Create<CatalogueItemId>();
                var item = new CatalogueItem
                {
                    Id = id,
                    PublishedStatus = PublicationStatus.Published,
                };

                AddCapabilities(item, context);
                AddPrices(item, context);
                InitializeSupplier(item, context);

                return item;
            }

            private static void AddCapabilities(CatalogueItem item, ISpecimenContext context)
            {
                var capabilities = context.CreateMany<CatalogueItemCapability>().ToList();
                capabilities.ForEach(c =>
                {
                    c.CatalogueItem = item;
                    c.CatalogueItemId = item.Id;
                    item.CatalogueItemCapabilities.Add(c);
                });
            }

            private static void AddPrices(CatalogueItem item, ISpecimenContext context)
            {
                var prices = context.CreateMany<CataloguePrice>().ToList();
                prices.ForEach(p =>
                {
                    p.CatalogueItem = item;
                    p.CatalogueItemId = item.Id;
                    item.CataloguePrices.Add(p);
                });
            }

            private static void InitializeSupplier(CatalogueItem item, ISpecimenContext context)
            {
                var supplier = context.Create<Supplier>();
                supplier.CatalogueItems.Add(item);
                supplier.Id = item.Id.SupplierId;

                item.Supplier = supplier;
                item.SupplierId = supplier.Id;
            }
        }
    }
}
