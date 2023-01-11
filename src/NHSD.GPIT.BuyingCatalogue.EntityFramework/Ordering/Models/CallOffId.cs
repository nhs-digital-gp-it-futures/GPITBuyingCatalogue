using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.TypeConverters;
using static System.FormattableString;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [TypeConverter(typeof(CallOffIdTypeConverter))]
    public readonly struct CallOffId : IEquatable<CallOffId>
    {
        public const int MaxOrderNumber = 999999;
        public const int MaxRevision = 99;

        private const string Pattern = @"^C(?<orderNumber>\d{1,6})-(?<revision>\d{1,2})$";

        private static readonly Lazy<Regex> Regex = new(() => new Regex(Pattern, RegexOptions.Compiled));

        [JsonConstructor]
        public CallOffId(int orderNumber, int revision)
        {
            if (orderNumber is < 0 or > MaxOrderNumber)
                throw new ArgumentOutOfRangeException(nameof(orderNumber), orderNumber, $"Value must be between 0 and {MaxOrderNumber}");

            if (revision > MaxRevision)
                throw new ArgumentOutOfRangeException(nameof(revision), revision, $"Value must be less than {MaxRevision}");

            OrderNumber = orderNumber;
            Revision = revision;
        }

        public int OrderNumber { get; }

        public int Revision { get; }

        public bool IsAmendment => Revision > 1;

        public static bool operator ==(CallOffId left, CallOffId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CallOffId left, CallOffId right)
        {
            return !(left == right);
        }

        public static (bool Success, CallOffId Id) Parse(string callOffId)
        {
            var match = Regex.Value.Match(callOffId);

            if (!match.Success)
            {
                return (false, default);
            }

            var orderNumber = int.Parse(match.Groups["orderNumber"].Value, CultureInfo.InvariantCulture);
            var revision = int.Parse(match.Groups["revision"].Value, CultureInfo.InvariantCulture);

            return (true, new CallOffId(orderNumber, revision));
        }

        public bool Equals(CallOffId other)
        {
            return OrderNumber == other.OrderNumber && Revision == other.Revision;
        }

        public override bool Equals(object obj)
        {
            return obj is CallOffId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (OrderNumber * 100) + Revision;
        }

        public override string ToString()
        {
            return Invariant($"C{OrderNumber:D6}-{Revision:D2}");
        }
    }
}
