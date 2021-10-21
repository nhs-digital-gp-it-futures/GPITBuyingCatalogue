using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class SupplierContactCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<SupplierContact> composer) => composer
                .Without(c => c.AssignedCatalogueItems)
                .Without(c => c.LastUpdatedByUser);

            fixture.Customize<SupplierContact>(ComposerTransformation);
        }
    }
}
