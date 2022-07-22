using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations.Contracts;

public class ContractCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Contract> composer) => composer
            .Without(c => c.ImplementationPlan)
            .Without(c => c.DataProcessingPlan)
            .Without(c => c.BillingItems);

        fixture.Customize<Contract>(ComposerTransformation);
    }
}
