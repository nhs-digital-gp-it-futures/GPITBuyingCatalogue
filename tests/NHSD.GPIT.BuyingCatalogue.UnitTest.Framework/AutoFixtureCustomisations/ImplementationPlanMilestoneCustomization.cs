using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class ImplementationPlanMilestoneCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ImplementationPlanMilestone> composer) => composer
            .Without(x => x.ContractBillingItem)
            .Without(x => x.ContractBillingItemId);

        fixture.Customize<ImplementationPlanMilestone>(ComposerTransformation);
    }
}
