using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class Gen2CapabilitiesCsvModelCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Gen2CapabilitiesCsvModel> composer) =>
            composer.Without(x => x.AdditionalServiceId)
                .With(x => x.SolutionId, fixture.Create<CatalogueItemId>().ToString())
                .With(x => x.CapabilityId, $"C{fixture.Create<uint>() % 100:D2}");

        fixture.Customize<Gen2CapabilitiesCsvModel>(ComposerTransformation);
    }
}
