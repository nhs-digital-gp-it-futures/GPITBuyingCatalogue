using AutoFixture;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class OrderCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var callOffId = fixture
                .Customize(new CallOffIdCustomization())
                .Create<CallOffId>();

            fixture.Customize<Order>(order => order
                .With(o => o.Id, callOffId.Id)
                .With(o => o.Revision, callOffId.Revision)
                .Without(o => o.IsDeleted)
                .Without(o => o.OrderStatus));
        }
    }
}
