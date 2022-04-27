using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class CatalogueItemCapabilityCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CatalogueItemCapability> composer) => composer
                .FromFactory(new CatalogueItemCapabilitySpecimenBuilder())
                .Without(cic => cic.CatalogueItem)
                .Without(cic => cic.CatalogueItemId)
                .Without(cic => cic.Capability)
                .Without(cic => cic.CapabilityId)
                .Without(cic => cic.LastUpdatedByUser);

            fixture.Customize<CatalogueItemCapability>(ComposerTransformation);
        }

        private sealed class CatalogueItemCapabilitySpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(CatalogueItemCapability)))
                    return new NoSpecimen();

                var capability = context.Create<Capability>();
                var itemCapability = new CatalogueItemCapability();

                capability.CatalogueItemCapabilities.Add(itemCapability);

                itemCapability.Capability = capability;
                itemCapability.CapabilityId = capability.Id;

                return itemCapability;
            }
        }
    }
}
