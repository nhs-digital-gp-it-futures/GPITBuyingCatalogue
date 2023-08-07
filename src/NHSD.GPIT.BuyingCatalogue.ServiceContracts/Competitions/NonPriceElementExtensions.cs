﻿using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public static class NonPriceElementExtensions
{
    public static bool HasNonPriceElement(this NonPriceElements nonPriceElements, NonPriceElement nonPriceElement)
    {
        if (nonPriceElements is null) return false;

        return nonPriceElement switch
        {
            NonPriceElement.Interoperability => nonPriceElements.Interoperability.Any(),
            NonPriceElement.Implementation => nonPriceElements.Implementation is not null,
            NonPriceElement.ServiceLevel => nonPriceElements.ServiceLevel is not null,
            _ => throw new ArgumentOutOfRangeException(
                nameof(nonPriceElement),
                nonPriceElement,
                "Invalid element specified"),
        };
    }

    public static int? GetNonPriceWeight(this NonPriceElements nonPriceElements, NonPriceElement nonPriceElement)
    {
        if (nonPriceElements?.NonPriceWeights is null) return null;

        return nonPriceElement switch
        {
            NonPriceElement.Implementation => nonPriceElements.NonPriceWeights.Implementation,
            NonPriceElement.Interoperability => nonPriceElements.NonPriceWeights.Interoperability,
            NonPriceElement.ServiceLevel => nonPriceElements.NonPriceWeights.ServiceLevel,
            _ => throw new ArgumentOutOfRangeException(
                nameof(nonPriceElement),
                nonPriceElement,
                "Invalid element specified"),
        };
    }

    public static bool HasIncompleteWeighting(this NonPriceElements nonPriceElements)
        => (nonPriceElements.NonPriceWeights != null) &&
            ((nonPriceElements.HasNonPriceElement(NonPriceElement.Interoperability) && nonPriceElements.GetNonPriceWeight(NonPriceElement.Interoperability) == null)
                || (nonPriceElements.HasNonPriceElement(NonPriceElement.Implementation) && nonPriceElements.GetNonPriceWeight(NonPriceElement.Implementation) == null)
                || (nonPriceElements.HasNonPriceElement(NonPriceElement.ServiceLevel) && nonPriceElements.GetNonPriceWeight(NonPriceElement.ServiceLevel) == null));

    public static IEnumerable<NonPriceElement> GetAvailableNonPriceElements(this NonPriceElements nonPriceElements) =>
        GetAllNonPriceElements().Where(x => !HasNonPriceElement(nonPriceElements, x)).ToArray();

    public static IEnumerable<NonPriceElement> GetNonPriceElements(this NonPriceElements nonPriceElements)
    {
        var allNonPriceElements = GetAllNonPriceElements();

        return allNonPriceElements.Except(nonPriceElements.GetAvailableNonPriceElements()).Order();
    }

    public static IEnumerable<NonPriceElement> GetAllNonPriceElements() =>
        Enum.GetValues<NonPriceElement>();
}
