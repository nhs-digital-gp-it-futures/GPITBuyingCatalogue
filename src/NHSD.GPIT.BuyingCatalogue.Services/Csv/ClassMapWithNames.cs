using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CsvHelper.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public abstract class ClassMapWithNames<T> : ClassMap<T>
    {
        private readonly ReadOnlyDictionary<string, string> names;

        protected ClassMapWithNames(ReadOnlyDictionary<string, string> names)
        {
            ArgumentNullException.ThrowIfNull(names);
            this.names = names;
        }

        protected ClassMapWithNames(IEnumerable<KeyValuePair<string, string>> defaultNames, IEnumerable<KeyValuePair<string, string>> overrideNames)
        {
            ArgumentNullException.ThrowIfNull(defaultNames);
            ArgumentNullException.ThrowIfNull(overrideNames);

            var names = new Dictionary<string, string>(defaultNames);

            foreach (var item in overrideNames)
            {
                names[item.Key] = item.Value;
            }

            this.names = names.AsReadOnly();
        }

        protected string GetName(string key)
        {
            if (names.TryGetValue(key, out string value))
            {
                return value;
            }

            return key;
        }
    }
}
