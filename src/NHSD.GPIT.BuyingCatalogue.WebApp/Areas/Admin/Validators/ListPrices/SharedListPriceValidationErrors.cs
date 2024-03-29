﻿namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    internal static class SharedListPriceValidationErrors
    {
        internal const string MissingTiersError = "Add at least 1 tier";
        internal const string InvalidStartingRangeError = "Lowest tier must have a low range of 1";
        internal const string InvalidEndingRangeError = "Highest tier must have an infinite upper range";
        internal const string RangeOverlapError = "Tier {0}'s upper range overlaps with Tier {1}'s lower range";
        internal const string RangeGapError = "There's a gap between Tier {0}'s upper range and Tier {1}'s lower range";
        internal const string SelectedProvisioningTypeError = "Select a provisioning type";
        internal const string SelectedCalculationTypeError = "Select a calculation type";
        internal const string UnitError = "Enter a unit";
        internal const string DuplicateListPriceError = "A list price with these details already exists";
        internal const string QuantitiesCalculationError = "Select how quantities are calculated";
        internal const string RangeDefinitionError = "Enter a range definition";
        internal const string UnitsError = "Enter a pluralised unit";
    }
}
