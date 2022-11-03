namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class CommonMemberAutoDataAttribute : MemberAutoDataAttribute
    {
        public CommonMemberAutoDataAttribute(string memberName, params object[] parameters)
            : base(memberName, parameters, FixtureFactory.Create)
        {
        }
    }
}
