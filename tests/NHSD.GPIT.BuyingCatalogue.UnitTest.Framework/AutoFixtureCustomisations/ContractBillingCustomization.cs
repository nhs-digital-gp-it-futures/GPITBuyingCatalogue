using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class ContractBillingCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ContractBilling> composer) => composer
            .Without(x => x.ContractBillingItems)
            .Without(x => x.Contract);

        fixture.Customize<ContractBilling>(ComposerTransformation);
    }
}
