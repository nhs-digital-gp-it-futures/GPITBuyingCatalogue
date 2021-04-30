using System;
using AutoMapper;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public class StringToBoolResolver : IMemberValueResolver<object, object, string, bool?>
    {
        public bool? Resolve(object source, object destination, string sourceMember, bool? destMember,
            ResolutionContext context)
        {
            return string.IsNullOrWhiteSpace(sourceMember)
                ? (bool?) null
                : sourceMember.Equals("Yes", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}