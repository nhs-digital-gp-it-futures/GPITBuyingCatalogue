
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations.Contracts;

public class DataProcessingPlanCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<DataProcessingPlan> composer) => composer
            .Without(c => c.Steps);

        fixture.Customize<DataProcessingPlan>(ComposerTransformation);
    }
}
