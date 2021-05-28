using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models
{
    public abstract class EnumerationBase : IComparable
    {
        protected EnumerationBase(int id, string name) => (Id, Name) = (id, name);

        public string Name { get; private set; }

        public int Id { get; private set; }

        public static IEnumerable<T> GetAll<T>()
             where T : EnumerationBase
        {
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(f => f.GetValue(null)).Cast<T>();
        }

        public static bool TryGetFromIdOrName<T>(
            string idOrName,
            out T enumeration)
            where T : EnumerationBase
        {
            return TryParse(item => item.Name == idOrName, out enumeration) ||
            (int.TryParse(idOrName, out var id) &&
            TryParse(item => item.Id == id, out enumeration));
        }

        public static T FromId<T>(int id)
            where T : EnumerationBase
        {
            var matchingItem = Parse<T, int>(id, "nameOrValue", item => item.Id == id);
            return matchingItem;
        }

        public static T FromName<T>(string name)
            where T : EnumerationBase
        {
            var matchingItem = Parse<T, string>(name, "name", item => item.Name == name);
            return matchingItem;
        }

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            if (obj is not EnumerationBase otherValue)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => Id.CompareTo(((EnumerationBase)other).Id);

        public override int GetHashCode() => Id.GetHashCode();

        private static bool TryParse<TEnumeration>(
        Func<TEnumeration, bool> predicate,
        out TEnumeration enumeration)
        where TEnumeration : EnumerationBase
        {
            enumeration = GetAll<TEnumeration>().FirstOrDefault(predicate);
            return enumeration != null;
        }

        private static TEnumeration Parse<TEnumeration, TIntOrString>(
        TIntOrString nameOrValue,
        string description,
        Func<TEnumeration, bool> predicate)
        where TEnumeration : EnumerationBase
        {
            var matchingItem = GetAll<TEnumeration>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                throw new InvalidOperationException(
                $"'{nameOrValue}' is not a valid {description} in {typeof(TEnumeration)}");
            }

            return matchingItem;
        }
    }
}
