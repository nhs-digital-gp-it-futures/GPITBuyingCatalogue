using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class TypeExtensions
    {
        public static string ControllerName(this Type type) => type.Name.Replace("Controller", null);

        public static string AreaName(this Type type) =>
            (type.GetCustomAttributes(typeof(AreaAttribute), false)?.FirstOrDefault() as AreaAttribute)?.RouteValue;
    }
}
