using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Extensions
{
    public static class ReflectionExtensions
    {
        public static T CopyObjectToNew<T>(this T input)
            where T : new()
        {
            var copy = new T();
            foreach (var item in input.GetType().GetProperties().Where(p => p.CanWrite))
            {
                copy.GetType().GetProperty(item.Name).SetValue(
                    copy,
                    item.GetValue(input, null),
                    null);
            }

            return copy;
        }

        public static void ValidateAllPropertiesExcept<T>(this T input, T toCheck, string[] propertiesToExclude)
        {
            var remainingProperties = input.GetType().GetProperties()
                .Where(mi => !propertiesToExclude.Contains(mi.Name));

            foreach (var propertyInfo in remainingProperties)
            {
                input.GetType().GetProperty(propertyInfo.Name).GetValue(toCheck)
                    .Should()
                    .BeEquivalentTo(typeof(ClientApplication).GetProperty(propertyInfo.Name).GetValue(input));
            }
        }
    }
}
