using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations.OdsOrganisations;

public class OrganisationRoleCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrganisationRole> composer) => composer
            .Without(x => x.Organisation)
            .Without(x => x.RoleType);

        fixture.Customize<OrganisationRole>(ComposerTransformation);
    }
}
