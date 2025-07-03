using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.TypeConverters;
using static System.FormattableString;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [TypeConverter(typeof(CatalogueItemIdTypeConverter))]
    public readonly struct CatalogueItemId : IEquatable<CatalogueItemId>
    {
        public const int MaxItemIdLength = 7;
        public const int MaxSupplierId = 999999;

        [JsonConstructor]
        public CatalogueItemId(int supplierId, string itemId)
        {
            if (supplierId is < 1 or > MaxSupplierId)
                throw new ArgumentOutOfRangeException(nameof(supplierId));

            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentException($"{nameof(itemId)} is required.", nameof(itemId));

            if (itemId.Length > MaxItemIdLength)
                throw new ArgumentOutOfRangeException(nameof(itemId));

            SupplierId = supplierId;
            ItemId = itemId;
        }

        public int SupplierId { get; }

        public string ItemId { get; }

        public static bool operator ==(CatalogueItemId left, CatalogueItemId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CatalogueItemId left, CatalogueItemId right)
        {
            return !(left == right);
        }

        public static bool TryParse(string catalogueItemId, out CatalogueItemId id)
        {
            id = default;

            if (string.IsNullOrWhiteSpace(catalogueItemId))
                return false;

            var catalogueId = catalogueItemId.AsSpan();
            var dashIndex = catalogueId.IndexOf('-');

            if (dashIndex == -1)
                return false;

            if (!int.TryParse(catalogueId[..dashIndex], out var supplierId))
                return false;

            var itemId = catalogueId[(dashIndex + 1)..];
            if (itemId.IsEmpty)
                return false;

            id = new CatalogueItemId(supplierId, itemId.ToString());

            return true;
        }

        public static CatalogueItemId ParseExact(string catalogueItemId)
        {
            if (!TryParse(catalogueItemId, out var id))
                throw new FormatException();

            return id;
        }

        public bool Equals(CatalogueItemId other)
        {
            return SupplierId == other.SupplierId && ItemId == other.ItemId;
        }

        public override bool Equals(object obj)
        {
            return obj is CatalogueItemId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SupplierId, ItemId);
        }

        public override string ToString()
        {
            return Invariant($"{SupplierId}-{ItemId}");
        }

        public CatalogueItemId NextSolutionId()
        {
            if (!int.TryParse(ItemId, out var itemId))
                throw new FormatException();

            return new CatalogueItemId(SupplierId, (itemId + 1).ToString("D3"));
        }

        public CatalogueItemId NextAssociatedServiceId()
        {
            const string associatedServiceIdPrefix = "S-";

            var itemIdSpan = ItemId.AsSpan();
            if (!itemIdSpan.StartsWith(associatedServiceIdPrefix) || !int.TryParse(
                    itemIdSpan[(itemIdSpan.IndexOf("-") + 1)..],
                    out var itemId))
                throw new FormatException();

            return new CatalogueItemId(SupplierId, $"{associatedServiceIdPrefix}{itemId + 1:D3}");
        }

        public CatalogueItemId NextAdditionalServiceId()
        {
            var itemIdSpan = ItemId.AsSpan();
            var catalogueItemId = itemIdSpan[..itemIdSpan.IndexOf('A')];
            var additionalServiceId = itemIdSpan[(itemIdSpan.IndexOf('A') + 1)..];

            if (!int.TryParse(additionalServiceId, out var itemId))
                throw new FormatException();

            var newItemId = $"{catalogueItemId}A{itemId + 1:D3}";
            return new CatalogueItemId(SupplierId, newItemId);
        }
    }
}
