using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Services.Identity
{
    public sealed class PasswordResetCallback : IPasswordResetCallback
    {
        private readonly IHttpContextAccessor accessor;
        private readonly LinkGenerator generator;
        private readonly DomainNameSettings domainNameSettings;

        public PasswordResetCallback(IHttpContextAccessor accessor, LinkGenerator generator, DomainNameSettings domainNameSettings)
        {
            this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
            this.domainNameSettings = domainNameSettings ?? throw new ArgumentNullException(nameof(domainNameSettings));
        }

        public Uri GetPasswordResetCallback(PasswordResetToken token)
        {
            token.ValidateNotNull(nameof(token));

            var action = generator.GetUriByAction(
                accessor.HttpContext,
                "ResetPassword",
                "Account",
                new { token.Token, token.User.Email, Area = "Identity" },
                "https",
                new HostString(domainNameSettings.DomainName));

            return new Uri(action);
        }
    }
}
