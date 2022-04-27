using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class ContactCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Contact> composer) => composer
                .Without(c => c.OrderOrderingPartyContacts)
                .Without(c => c.OrderSupplierContacts);

            fixture.Customize<Contact>(ComposerTransformation);
        }
    }
}
