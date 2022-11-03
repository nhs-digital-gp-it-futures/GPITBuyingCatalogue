using System;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class InMemoryDbMemberAutoDataAttribute : MemberAutoDataAttribute
    {
        public InMemoryDbMemberAutoDataAttribute(string memberName, params object[] parameters)
            : base(
                memberName,
                parameters,
                FixtureFactory.Create(
                    new BuyingCatalogueDbContextCustomization(),
                    new InMemoryDbCustomization(Guid.NewGuid().ToString()),
                    new UserManagerCustomization()))
        {
        }
    }
}
