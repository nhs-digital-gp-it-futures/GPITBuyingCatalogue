using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class Gen2CapabilityMappingModelCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(IFactoryComposer<Gen2CapabilityMappingModel> composer) =>
            composer.FromFactory(new Gen2CapabilityMappingModelSpecimenBuilder());

        fixture.Customize<Gen2CapabilityMappingModel>(ComposerTransformation);
    }

    private sealed class Gen2CapabilityMappingModelSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!(request as Type == typeof(Gen2CapabilityMappingModel)))
                return new NoSpecimen();

            var id = context.Create<int>();
            var epics = context.CreateMany<string>();

            var model = new Gen2CapabilityMappingModel($"C{id}", epics);

            return model;
        }
    }
}
