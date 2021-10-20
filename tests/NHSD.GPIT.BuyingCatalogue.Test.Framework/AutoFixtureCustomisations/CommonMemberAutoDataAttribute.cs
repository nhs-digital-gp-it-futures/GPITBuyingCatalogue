using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CommonMemberAutoDataAttribute : MemberAutoDataAttribute
    {
        public CommonMemberAutoDataAttribute(string memberName, params object[] parameters)
            : base(new CommonAutoDataAttribute(), memberName, parameters)
        {
        }
    }
}
