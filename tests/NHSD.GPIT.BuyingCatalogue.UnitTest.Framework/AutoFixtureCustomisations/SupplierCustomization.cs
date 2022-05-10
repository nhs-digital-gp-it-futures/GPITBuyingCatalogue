using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class SupplierCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Supplier> c) => c
                .Without(s => s.CatalogueItems)
                .Without(s => s.LastUpdatedByUser);

            fixture.Customize<Supplier>(ComposerTransformation);
        }
    }
}
