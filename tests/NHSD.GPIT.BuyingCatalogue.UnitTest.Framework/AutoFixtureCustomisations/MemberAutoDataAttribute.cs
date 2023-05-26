﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using Xunit;
using Xunit.Sdk;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

/// <summary>
/// AutoFixture's MemberAutoDataAttribute doesn't work correctly as it retrieves only one record from the test data.
/// This is a temporary fix until AutoFixture's xUnit provider correctly supports MemberAutoData in version 5.
/// https://github.com/AutoFixture/AutoFixture/issues/1142.
/// https://github.com/AutoFixture/AutoFixture/discussions/1327.
/// </summary>
public class MemberAutoDataAttribute : DataAttribute
{
    private readonly Lazy<IFixture> fixture;
    private readonly MemberDataAttribute memberDataAttribute;

    public MemberAutoDataAttribute(string memberName, params object[] parameters)
        : this(memberName, parameters, () => new Fixture())
    {
    }

    protected MemberAutoDataAttribute(string memberName, object[] parameters, Func<IFixture> fixtureFactory)
    {
        if (fixtureFactory == null)
        {
            throw new ArgumentNullException(nameof(fixtureFactory));
        }

        memberDataAttribute = new(memberName, parameters);
        fixture = new(fixtureFactory, LazyThreadSafetyMode.PublicationOnly);
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (testMethod == null)
        {
            throw new ArgumentNullException(nameof(testMethod));
        }

        var memberData = memberDataAttribute.GetData(testMethod);

        using var enumerator = memberData.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var specimens = GetSpecimens(testMethod.GetParameters(), enumerator.Current!.Length).ToArray();

        do
        {
            yield return enumerator.Current!.Concat(specimens).ToArray();
        }
        while (enumerator.MoveNext());
    }

    private IEnumerable<object> GetSpecimens(IEnumerable<ParameterInfo> parameters, int skip)
    {
        foreach (var parameter in parameters.Skip(skip))
        {
            CustomizeFixture(parameter);

            yield return Resolve(parameter);
        }
    }

    private void CustomizeFixture(ParameterInfo p)
    {
        var customizeAttributes = p.GetCustomAttributes()
            .OfType<IParameterCustomizationSource>()
            .OrderBy(x => x, new CustomizeAttributeComparer());

        foreach (var ca in customizeAttributes)
        {
            var c = ca.GetCustomization(p);
            fixture.Value.Customize(c);
        }
    }

    private object Resolve(ParameterInfo p)
    {
        var context = new SpecimenContext(fixture.Value);

        return context.Resolve(p);
    }

    private class CustomizeAttributeComparer : Comparer<IParameterCustomizationSource>
    {
        public override int Compare(IParameterCustomizationSource x, IParameterCustomizationSource y)
        {
            var xfrozen = x is FrozenAttribute;
            var yfrozen = y is FrozenAttribute;

            if (xfrozen && !yfrozen)
            {
                return 1;
            }

            if (yfrozen && !xfrozen)
            {
                return -1;
            }

            return 0;
        }
    }
}
