using AutoMapper;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class StringToNullableBoolResolver : IMemberValueResolver<object, object, string, bool?>,
        ITypeConverter<string, bool?>
    {
        public bool? Convert(string source, bool? destination, ResolutionContext context) => GetValue(source);

        public bool? Resolve(object source, object destination, string sourceMember, bool? destMember,
            ResolutionContext context) => GetValue(sourceMember);

        private static bool? GetValue(string sourceMember) =>
            string.IsNullOrWhiteSpace(sourceMember)
                ? (bool?) null
                : sourceMember.EqualsIgnoreCase("Yes");
    }
}