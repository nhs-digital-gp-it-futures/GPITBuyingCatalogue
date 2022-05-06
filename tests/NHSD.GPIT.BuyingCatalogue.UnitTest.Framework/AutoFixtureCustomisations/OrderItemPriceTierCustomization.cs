using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class OrderItemPriceTierCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrderItemPriceTier> composer) => composer
                .Without(t => t.CatalogueItemId)
                .Without(t => t.LowerRange)
                .Without(t => t.UpperRange)
                .Without(t => t.OrderId)
                .Without(t => t.OrderItemPrice);

            fixture.Customize<OrderItemPriceTier>(ComposerTransformation);
        }
    }
}
