using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Services.Identity
{
    public sealed class PasswordResetCallback : IPasswordResetCallback
    {
        private readonly IHttpContextAccessor accessor;
        private readonly LinkGenerator generator;

        public PasswordResetCallback(IHttpContextAccessor accessor, LinkGenerator generator)
        {
            this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public Uri GetPasswordResetCallback(PasswordResetToken token)
        {
            token.ValidateNotNull(nameof(token));

            var context = accessor.HttpContext;
            var hostString = new HostString(GetAuthority());

            var action = generator.GetUriByAction(
                context,
                "ResetPassword",
                "Account",
                new { token.Token, token.User.Email, Area = "Identity" },
                "https",
                hostString);

            return new Uri(action);
        }

        private string GetAuthority()
        {
            if (accessor.HttpContext.Request.Host.Port.HasValue)
                return $"{accessor.HttpContext.Request.Host.Host}:{accessor.HttpContext.Request.Host.Port}";

            return accessor.HttpContext.Request.Host.Host;
        }
    }
}
