using AutoFixture.Xunit2;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class CommomMemberAutoDataAttribute : MemberAutoDataAttribute
    {
        public CommomMemberAutoDataAttribute(string memberName, params object[] parameters)
            : base(new CommonAutoDataAttribute(), memberName, parameters)
        {
        }
    }
}
