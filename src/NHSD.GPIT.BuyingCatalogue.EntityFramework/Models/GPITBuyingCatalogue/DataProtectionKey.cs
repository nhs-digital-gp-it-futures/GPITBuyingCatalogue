﻿using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    [ExcludeFromCodeCoverage(Justification = "Class currently only contains automatic properties")]
    public sealed class DataProtectionKey
    {
        public int Id { get; set; }

        public string FriendlyName { get; set; }

        public string Xml { get; set; }
    }
}
