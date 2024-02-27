using System;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class InMemoryDbMemberAutoDataAttribute : MemberAutoDataAttribute
    {
        public InMemoryDbMemberAutoDataAttribute(string memberName, MockingFramework mockingFramework = MockingFramework.Moq, params object[] parameters)
            : base(
                memberName,
                parameters,
                () => FixtureFactory.Create(
                    mockingFramework,
                    new BuyingCatalogueDbContextCustomization(),
                    new InMemoryDbCustomization(Guid.NewGuid().ToString(), mockingFramework),
                    new UserManagerCustomization()))
        {
        }
    }
}
