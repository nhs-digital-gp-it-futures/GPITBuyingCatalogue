using AutoFixture;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class OrderEntityCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Order>(
                o =>
                o.With(o => o.Id, (fixture.Create<int>() % CallOffId.MaxId) + 1)
                .With(o => o.Revision, (fixture.Create<byte>() % CallOffId.MaxRevision) + 1));
        }
    }
}
