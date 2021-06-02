using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class FundingSourceModel : OrderingBaseModel
    {
        public FundingSourceModel()
        {
        }

        public FundingSourceModel(string odsCode, string callOffId, bool? fundingSourceOnlyGms)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}";
            Title = $"Funding source for {callOffId}";
            FundingSourceOnlyGms = fundingSourceOnlyGms.ToYesNo();
        }

        [Required(ErrorMessage = "Select yes if you're paying for this order in full using your GP IT Futures centrally held funding allocation")]
        public string FundingSourceOnlyGms { get; set; }
    }
}
