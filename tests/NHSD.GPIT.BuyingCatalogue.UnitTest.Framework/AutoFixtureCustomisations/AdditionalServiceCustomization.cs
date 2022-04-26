using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class AdditionalServiceCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<AdditionalService> composer) => composer
                .FromFactory(new AdditionalServiceSpecimenBuilder())
                .Without(a => a.CatalogueItem)
                .Without(a => a.CatalogueItemId)
                .Without(a => a.LastUpdatedByUser)
                .Without(a => a.Solution);

            fixture.Customize<AdditionalService>(ComposerTransformation);
        }

        private sealed class AdditionalServiceSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(AdditionalService)))
                    return new NoSpecimen();

                var catalogueItem = context.Create<CatalogueItem>();
                var service = new AdditionalService();

                catalogueItem.AdditionalService = service;
                catalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;

                service.CatalogueItem = catalogueItem;
                service.CatalogueItemId = catalogueItem.Id;

                return service;
            }
        }
    }
}
