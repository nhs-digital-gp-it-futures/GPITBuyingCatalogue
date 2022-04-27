using AutoFixture;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class OrderCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Order>(order => order
                .Without(o => o.DefaultDeliveryDates)
                .Without(o => o.IsDeleted)
                .Without(o => o.LastUpdatedByUser)
                .Without(o => o.OrderStatus));
        }
    }
}
