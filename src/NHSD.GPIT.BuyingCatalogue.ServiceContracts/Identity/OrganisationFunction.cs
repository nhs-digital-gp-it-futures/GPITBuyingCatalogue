using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity
{
    public sealed class OrganisationFunction : IEquatable<OrganisationFunction>
    {
        public const string AuthorityName = "Authority";
        public const string BuyerName = "Buyer";

        public static readonly OrganisationFunction Authority = new(1, AuthorityName);
        public static readonly OrganisationFunction Buyer = new(2, BuyerName);

        private static readonly IEnumerable<OrganisationFunction> Values = new[] { Authority, Buyer };

        private OrganisationFunction(int value, string displayName)
        {
            Value = value;
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        public int Value { get; }

        public string DisplayName { get; }

        public static OrganisationFunction FromDisplayName(string displayName)
        {
            if (displayName is null)
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            return Values.SingleOrDefault(o => string.Equals(o.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as OrganisationFunction);
        }

        public bool Equals(OrganisationFunction other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        public override int GetHashCode() => HashCode.Combine(Value);
    }
}
