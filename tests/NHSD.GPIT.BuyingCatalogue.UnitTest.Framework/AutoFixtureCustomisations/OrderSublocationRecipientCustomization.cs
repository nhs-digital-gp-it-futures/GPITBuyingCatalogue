using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class OrderSublocationRecipientCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrderSublocationRecipient> composer)
            {
                return composer
                    .Without(x => x.OrderItemSublocationRecipients);
            }

            fixture.Customize<OrderSublocationRecipient>(ComposerTransformation);
        }
    }
}
