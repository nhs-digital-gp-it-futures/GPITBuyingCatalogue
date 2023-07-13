using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class ContractBillingItemCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ContractBillingItem> composer) => composer
            .Without(x => x.OrderItem)
            .Without(x => x.OrderId);

        fixture.Customize<ContractBillingItem>(ComposerTransformation);
    }
}
