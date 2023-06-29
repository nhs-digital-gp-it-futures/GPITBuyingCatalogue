using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations.OdsOrganisations;

public class OrganisationRelationshipCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrganisationRelationship> composer) =>
            composer
                .Without(x => x.TargetOrganisation)
                .Without(x => x.OwnerOrganisation)
                .Without(x => x.RelationshipType);

        fixture.Customize<OrganisationRelationship>(ComposerTransformation);
    }
}
