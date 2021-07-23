using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Services.Identity
{
    public sealed class PasswordResetCallback : IPasswordResetCallback
    {
        private readonly IHttpContextAccessor accessor;
        private readonly LinkGenerator generator;
        private readonly ILogger<PasswordResetCallback> logger;

        public PasswordResetCallback(IHttpContextAccessor accessor, LinkGenerator generator, ILogger<PasswordResetCallback> logger)
        {
            this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            this.generator = generator ?? throw new ArgumentNullException(nameof(generator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            foreach (var header in accessor.HttpContext.Request.Headers)
                logger.LogInformation($"Header: {header.Key} {header.Value}");

            if (accessor.HttpContext.Request.Host.Port.HasValue && accessor.HttpContext.Request.Host.Port.Value != 80)
                return $"{accessor.HttpContext.Request.Host.Host}:{accessor.HttpContext.Request.Host.Port}";

            return accessor.HttpContext.Request.Host.Host;
        }
    }
}
