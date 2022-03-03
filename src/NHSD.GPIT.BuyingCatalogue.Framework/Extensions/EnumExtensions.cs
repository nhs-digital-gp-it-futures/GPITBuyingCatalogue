namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsCentralFunding(this ServiceContracts.Enums.FundingSource fundingSource)
            => fundingSource == ServiceContracts.Enums.FundingSource.Central;
    }
}
