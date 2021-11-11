using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class StandardCapabilityCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<StandardCapability> composer) => composer
                .FromFactory(new StandardCapabilitySpecimenBuilder())
                .Without(sc => sc.Capability)
                .Without(sc => sc.CapabilityId)
                .Without(sc => sc.Standard)
                .Without(sc => sc.StandardId);

            fixture.Customize<StandardCapability>(ComposerTransformation);
        }

        private sealed class StandardCapabilitySpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(StandardCapability)))
                    return new NoSpecimen();

                var standard = context.Create<Standard>();
                var standardCapability = new StandardCapability();

                standardCapability.Standard = standard;
                standardCapability.StandardId = standard.Id;

                standard.StandardCapabilities.Add(standardCapability);

                return standardCapability;
            }
        }
    }
}
