using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class InMemoryDbMemberAutoDataAttribute : MemberAutoDataAttribute
    {
        public InMemoryDbMemberAutoDataAttribute(string memberName, params object[] parameters)
            : base(new InMemoryDbAutoDataAttribute(), memberName, parameters)
        {
        }
    }
}
