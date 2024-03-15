using System;
using AutoFixture;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;

/// <summary>
/// Prevents Reflection from instantiating an implementation of <see cref="ICustomization"/>.
/// </summary>
/// <remarks>
/// This should only be used on concrete types that implement <see cref="ICustomization"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
internal class ExcludesAutoCustomizationAttribute : Attribute
{
}
