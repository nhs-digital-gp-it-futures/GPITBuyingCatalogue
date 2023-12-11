using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email
{
    [ExcludeFromCodeCoverage]
    public sealed record EmailAddressTemplate
    {
        private readonly string address;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressTemplate"/> class.
        /// </summary>
        /// <remarks>Required for deserialization.</remarks>
        public EmailAddressTemplate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressTemplate"/> class
        /// with the specified <paramref name="address"/> and <paramref name="displayName"/>.
        /// </summary>
        /// <param name="address">The actual e-mail address.</param>
        /// <param name="displayName">An optional display name.</param>
        public EmailAddressTemplate(string address, string displayName = null)
        {
            Address = address;
            DisplayName = displayName;
        }

        /// <summary>
        /// Gets the actual e-mail address.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/>, empty or
        /// white space.</exception>
        public string Address
        {
            get => address;
            init
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(
                        $"{nameof(value)} cannot be null, empty or white space.",
                        nameof(value));
                }

                address = value;
            }
        }

        /// <summary>
        /// Gets the display name of the address.
        /// </summary>
        /// <remarks>An optional display name, for example Buying Catalogue Team.</remarks>
        public string DisplayName { get; init; }

        /// <summary>
        /// Returns the <see cref="EmailAddress"/> representation of the current instance.
        /// </summary>
        /// <returns>The <see cref="EmailAddress"/> representation of the current instance.</returns>
        public EmailAddress AsEmailAddress() => Address is null
            ? null
            : new EmailAddress(Address, DisplayName);
    }
}
