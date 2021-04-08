using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email
{
    /// <summary>
    /// An e-mail address.
    /// </summary>
    public sealed class EmailAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddress"/> class
        /// with the given display name and address.
        /// </summary>
        /// <param name="address">The actual e-mail address.</param>
        /// <param name="displayName">An optional display name.</param>
        /// <exception cref="ArgumentException"><paramref name="address"/> is <see langword="null"/>, empty or consists
        /// only of white space.</exception>
        public EmailAddress(string address, string? displayName = null)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException($"{nameof(address)} must be provided", nameof(address));

            Address = address;
            DisplayName = displayName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddress"/> class
        /// using the provided <paramref name="addressTemplate"/>.
        /// </summary>
        /// <param name="addressTemplate">The actual e-mail address.</param>
        /// <exception cref="ArgumentNullException"><paramref name="addressTemplate"/> is
        /// <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><see cref="EmailAddressTemplate.Address"/>
        /// of <paramref name="addressTemplate"/> is <see langword="null"/>.</exception>
        public EmailAddress(EmailAddressTemplate addressTemplate)
        {
            if (addressTemplate is null)
                throw new ArgumentNullException(nameof(addressTemplate));

            Address = addressTemplate.Address ?? throw new ArgumentException(
                $"{nameof(EmailAddressTemplate.Address)} of {nameof(addressTemplate)} must be provided",
                nameof(addressTemplate));

            DisplayName = addressTemplate.DisplayName;
        }

        /// <summary>
        /// Gets the display name of the address.
        /// </summary>
        /// <remarks>An optional display name, for example Buying Catalogue Team.</remarks>
        public string? DisplayName { get; }

        /// <summary>
        /// Gets the actual e-mail address.
        /// </summary>
        public string Address { get; }
    }
}
