using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations.OdsOrganisations;

public class OdsOrganisationCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OdsOrganisation> composer) => composer
            .Without(x => x.Related)
            .Without(x => x.Roles)
            .Without(x => x.Parents);

        fixture.Customize<OdsOrganisation>(ComposerTransformation);
    }
}
