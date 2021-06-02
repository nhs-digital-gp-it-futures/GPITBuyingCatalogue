using System;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class TypeExtensions
    {
        public static string ControllerName(this Type type) => type.Name.Replace("Controller", null);
    }
}
