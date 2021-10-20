using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CapabilityCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Epic>(c => c.Without(e => e.CapabilityId));

            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Capability> composer) => composer
                .FromFactory(new CapabilitySpecimenBuilder())
                .Without(c => c.CatalogueItemCapabilities)
                .Without(c => c.Category)
                .Without(c => c.CategoryId)
                .Without(c => c.Epics)
                .Without(c => c.FrameworkCapabilities)
                .Without(c => c.Id);

            fixture.Customize<Capability>(ComposerTransformation);
        }

        private sealed class CapabilitySpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(Capability)))
                    return new NoSpecimen();

                var capability = new Capability();
                var capabilityCategory = context.Create<CapabilityCategory>();
                var epics = context.CreateMany<Epic>().ToList();
                var id = context.Create<int>();

                capability.Id = id;
                capability.Category = capabilityCategory;
                capability.CategoryId = capabilityCategory.Id;

                epics.ForEach(e =>
                {
                    capability.Epics.Add(e);
                    e.CapabilityId = id;
                });

                return capability;
            }
        }
    }
}
