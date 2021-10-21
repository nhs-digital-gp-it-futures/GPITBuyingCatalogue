using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class AssociatedServiceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<AssociatedService> composer) => composer
                .FromFactory(new AssociatedServiceSpecimenBuilder())
                .Without(a => a.LastUpdatedByUser)
                .Without(a => a.CatalogueItem)
                .Without(a => a.CatalogueItemId);

            fixture.Customize<AssociatedService>(ComposerTransformation);
        }

        private sealed class AssociatedServiceSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(AssociatedService)))
                    return new NoSpecimen();

                var catalogueItem = context.Create<CatalogueItem>();
                var service = new AssociatedService();

                catalogueItem.AssociatedService = service;
                catalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

                service.CatalogueItem = catalogueItem;
                service.CatalogueItemId = catalogueItem.Id;

                return service;
            }
        }
    }
}
