using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public sealed class TermsOfUseModel
    {
        public TermsOfUseModel()
        {
        }

        public TermsOfUseModel(
            AspNetUser user,
            IEnumerable<string> userRoles,
            DateTime termsOfUseRevisionDate)
        {
            if (user is null)
                return;

            HasAcceptedTermsOfUse
                = HasAcceptedPrivacyPolicy
                = AlreadyAcceptedLatestTerms
                = user.HasAcceptedLatestTermsOfUse(termsOfUseRevisionDate);

            IsBuyer = userRoles.Any(
                r => string.Equals(OrganisationFunction.Buyer.Name, r, StringComparison.OrdinalIgnoreCase));

            HasOptedInUserResearch = user.HasOptedInUserResearch;
        }

        public bool HasAcceptedTermsOfUse { get; set; }

        public bool HasAcceptedPrivacyPolicy { get; set; }

        public bool HasOptedInUserResearch { get; set; }

        public bool AlreadyAcceptedLatestTerms { get; set; }

        public bool IsBuyer { get; set; }

        public bool IsAuthenticated { get; set; }

        public string ReturnUrl { get; set; }
    }
}
