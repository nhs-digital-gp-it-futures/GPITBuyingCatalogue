using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class OrganisationCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Organisation> c) =>
                c.Without(o => o.Orders)
                    .Without(o => o.RelatedOrganisationOrganisations)
                    .Without(o => o.RelatedOrganisationRelatedOrganisationNavigations);

            fixture.Customize<Organisation>(ComposerTransformation);
        }
    }
}
