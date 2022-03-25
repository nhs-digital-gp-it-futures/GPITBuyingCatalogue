using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public sealed class TermsOfUseModel
    {
        public TermsOfUseModel()
        {
        }

        public TermsOfUseModel(AspNetUser user, DateTime termsOfUseRevisionDate)
        {
            if (user is null)
                return;

            HasAcceptedTermsOfUse
                = HasAcceptedPrivacyPolicy
                = AlreadyAcceptedLatestTerms
                = user.HasAcceptedLatestTermsOfUse(termsOfUseRevisionDate);

            IsBuyer = user.IsBuyer();

            HasOptedInUserResearch = user.HasOptedInUserResearch;
        }

        public bool HasAcceptedTermsOfUse { get; set; }

        public bool HasAcceptedPrivacyPolicy { get; set; }

        public bool HasOptedInUserResearch { get; set; }

        public bool AlreadyAcceptedLatestTerms { get; set; }

        public bool IsBuyer { get; set; }

        public bool ShouldShowCheckboxes => IsBuyer && !AlreadyAcceptedLatestTerms;

        public bool IsAuthenticated { get; set; }

        public string ReturnUrl { get; set; }
    }
}
