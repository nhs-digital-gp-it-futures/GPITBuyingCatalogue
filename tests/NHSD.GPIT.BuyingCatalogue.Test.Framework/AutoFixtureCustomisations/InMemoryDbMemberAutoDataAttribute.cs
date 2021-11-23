using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class InMemoryDbMemberAutoDataAttribute : MemberAutoDataAttribute
    {
        public InMemoryDbMemberAutoDataAttribute(string memberName, params object[] parameters)
            : base(new InMemoryDbAutoDataAttribute(), memberName, parameters)
        {
        }
    }
}
