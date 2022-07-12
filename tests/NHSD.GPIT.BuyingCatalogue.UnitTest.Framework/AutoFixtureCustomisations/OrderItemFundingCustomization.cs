using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class OrderItemFundingCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<OrderItemFunding> composer) => composer
                .Without(oif => oif.CatalogueItemId)
                .Without(oif => oif.OrderId)
                .Without(oif => oif.OrderItem);

            fixture.Customize<OrderItemFunding>(ComposerTransformation);
        }
    }
}
