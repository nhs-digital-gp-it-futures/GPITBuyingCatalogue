using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity
{
    public sealed class OrganisationFunction : IEquatable<OrganisationFunction>
    {
        public static readonly OrganisationFunction Authority = new(1, "Authority", "Admin");
        public static readonly OrganisationFunction Buyer = new(2, "Buyer", "Buyer");
        public static readonly OrganisationFunction AccountManager = new(3, "AccountManager", "Account Manager");

        private static readonly IEnumerable<OrganisationFunction> Values = new[] { Authority, Buyer, AccountManager };

        private OrganisationFunction(int value, string name, string displayName)
        {
            Value = value;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        public int Value { get; }

        public string Name { get; }

        public string DisplayName { get; }

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
