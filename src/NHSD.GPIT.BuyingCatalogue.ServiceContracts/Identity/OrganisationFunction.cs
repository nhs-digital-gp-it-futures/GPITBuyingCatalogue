using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity
{
    public sealed class OrganisationFunction : IEquatable<OrganisationFunction>
    {
        private const string AuthorityAdvice = "This type of user is an administrator on behalf of NHS Digital.";

        private const string BuyerAdvice =
            "This type of user can place orders on the Buying Catalogue{0}.";

        private const string AccountManagerAdvice = "This type of user can manage user accounts for {0} organisation.";

        public static readonly OrganisationFunction Authority = new(1, "Authority", "Admin", AuthorityAdvice, AuthorityAdvice);
        public static readonly OrganisationFunction Buyer = new(2, "Buyer", "Buyer", string.Format(BuyerAdvice, " for your organisation"), string.Format(BuyerAdvice, string.Empty));
        public static readonly OrganisationFunction AccountManager = new(3, "AccountManager", "Account manager", string.Format(AccountManagerAdvice, "your"), string.Format(AccountManagerAdvice, "a buyer"));

        private static readonly IEnumerable<OrganisationFunction> Values = new[] { Authority, Buyer, AccountManager };

        private OrganisationFunction(int value, string name, string displayName, string advice, string internalAdvice)
        {
            Value = value;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Advice = advice;
            InternalAdvice = internalAdvice;
        }

        public int Value { get; }

        public string Name { get; }

        public string DisplayName { get; }

        public string Advice { get; }

        public string InternalAdvice { get; }

        public static OrganisationFunction FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return Values.FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));
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
