using System;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Models
{
    public class SelectOption<TValue> : IEquatable<SelectOption<TValue>>
    {
        public SelectOption()
        {
        }

        public SelectOption(string text, TValue value)
        {
            Text = text;
            Value = value;
            Advice = null;
            Selected = false;
        }

        public SelectOption(string text, TValue value, bool selected)
        {
            Text = text;
            Value = value;
            Advice = null;
            Selected = selected;
        }

        public SelectOption(string text, string advice, TValue value)
        {
            Text = text;
            Advice = advice;
            Value = value;
            Selected = false;
        }

        public string Text { get; set; }

        public string Advice { get; set; }

        public TValue Value { get; set; }

        public bool Selected { get; set; }

        public static bool operator ==(SelectOption<TValue> left, SelectOption<TValue> right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(SelectOption<TValue> left, SelectOption<TValue> right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is SelectOption<TValue> s && Equals(s);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals(SelectOption<TValue> other)
        {
            if (other == null)
            {
                return false;
            }

            return (Text ?? string.Empty).Equals(other.Text ?? string.Empty, StringComparison.Ordinal)
                && (Advice ?? string.Empty).Equals(other.Advice ?? string.Empty, StringComparison.Ordinal)
                && (Value?.ToString() ?? string.Empty).Equals(other.Value?.ToString() ?? string.Empty, StringComparison.Ordinal);
        }
    }
}
