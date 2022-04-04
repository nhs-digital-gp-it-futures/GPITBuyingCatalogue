using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource
{
    public sealed class ConfirmFundingSourceModel : FundingSourceModel
    {
        public ConfirmFundingSourceModel()
        {
        }

        public ConfirmFundingSourceModel(CallOffId callOffId, bool? fundingSourceOnlyGms = false)
        {
            CallOffId = callOffId;

            if (fundingSourceOnlyGms.HasValue)
                SelectedFundingSource = fundingSourceOnlyGms!.Value ? ServiceContracts.Enums.FundingSource.Central : ServiceContracts.Enums.FundingSource.Local;
        }

        public CallOffId CallOffId { get; set; }
    }
}
