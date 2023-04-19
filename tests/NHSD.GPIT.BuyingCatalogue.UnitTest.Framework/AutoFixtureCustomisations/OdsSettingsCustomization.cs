using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class OdsSettingsCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OdsSettings> composer) => composer
                .With(d => d.BuyerOrganisationRoles, new List<OrganisationRoleSettings>()
                {
                    new OrganisationRoleSettings { PrimaryRoleId = "RO213" },
                    new OrganisationRoleSettings { PrimaryRoleId = "RO261", SecondaryRoleId = "RO318", },
                    new OrganisationRoleSettings { PrimaryRoleId = "RO177", SecondaryRoleId = "RO76", },
                });

            fixture.Customize<OdsSettings>(ComposerTransformation);
        }
    }
}
