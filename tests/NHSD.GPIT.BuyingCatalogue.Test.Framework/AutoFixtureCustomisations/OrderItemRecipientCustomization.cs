using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class OrderItemRecipientCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrderItemRecipient> composer) => composer
                .Without(r => r.OrderItem)
                .Without(r => r.Recipient)
                .Without(r => r.CatalogueItemId)
                .Without(r => r.OrderId);

            fixture.Customize<OrderItemRecipient>(ComposerTransformation);
        }
    }
}
