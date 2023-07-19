using System;
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
            _ => throw new ArgumentOutOfRangeException(nameof(nonPriceElement), nonPriceElement, "Invalid element specified"),
        };
    }

    public static IEnumerable<NonPriceElement> GetAvailableNonPriceElements(this NonPriceElements nonPriceElements) =>
        GetAllNonPriceElements(nonPriceElements).Where(x => !HasNonPriceElement(nonPriceElements, x)).ToArray();

    public static IEnumerable<NonPriceElement> GetAllNonPriceElements(this NonPriceElements nonPriceElements) =>
        Enum.GetValues<NonPriceElement>();
}
