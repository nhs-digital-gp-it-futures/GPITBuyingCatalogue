using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class CapabilityCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Capability> composer) => composer
                .FromFactory(new CapabilitySpecimenBuilder())
                .Without(c => c.CatalogueItemCapabilities)
                .Without(c => c.Category)
                .Without(c => c.CategoryId)
                .Without(c => c.Epics)
                .Without(c => c.CapabilityEpics)
                .Without(c => c.StandardCapabilities)
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
                var id = context.Create<int>();

                capability.Id = id;
                capability.Category = capabilityCategory;
                capability.CategoryId = capabilityCategory.Id;

                AddEpics(capability, context);
                AddStandards(capability, context);

                return capability;
            }

            private static void AddStandards(Capability item, ISpecimenContext context)
            {
                var standards = context.CreateMany<StandardCapability>().ToList();
                standards.ForEach(sc =>
                {
                    sc.Capability = item;
                    sc.CapabilityId = item.Id;
                    item.StandardCapabilities.Add(sc);
                });
            }

            private static void AddEpics(Capability item, ISpecimenContext context)
            {
                var epics = context.CreateMany<Epic>().ToList();
                epics.ForEach(e =>
                {
                    item.Epics.Add(e);
                    e.Capabilities = new List<Capability> { item };
                });
            }
        }
    }
}
