using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal class CatalogueItemEpicCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CatalogueItemEpic> composer) => composer
                .Without(e => e.CatalogueItem)
                .Without(e => e.CatalogueItemId)
                .Without(e => e.Status)
                .Without(e => e.Epic)
                .Without(e => e.CapabilityId)
                .Without(e => e.LastUpdatedByUser);

            fixture.Customize<CatalogueItemEpic>(ComposerTransformation);
        }
    }
}
