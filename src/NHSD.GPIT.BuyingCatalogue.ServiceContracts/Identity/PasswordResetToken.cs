﻿using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity
{
    /// <summary>
    /// Token information for resetting a user's password.
    /// </summary>
    public sealed class PasswordResetToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordResetToken"/> class
        /// with the specified <paramref name="token"/> and <paramref name="user"/>.
        /// </summary>
        /// <param name="token">The token generated by the Identity framework.</param>
        /// <param name="user">The user requesting a password reset.</param>
        public PasswordResetToken(string token, AspNetUser user)
        {
            Token = token;
            User = user;
        }

        /// <summary>
        /// Gets the Identity password reset token.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        public AspNetUser User { get; }
    }
}
