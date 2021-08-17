using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Constants
{
    public static class SelectLists
    {
        public static readonly List<SelectListItem> MemorySizes = new()
        {
            new SelectListItem { Text = "256MB", Value = "256MB" },
            new SelectListItem { Text = "512MB", Value = "512MB" },
            new SelectListItem { Text = "1GB", Value = "1GB" },
            new SelectListItem { Text = "2GB", Value = "2GB" },
            new SelectListItem { Text = "4GB", Value = "4GB" },
            new SelectListItem { Text = "8GB", Value = "8GB" },
            new SelectListItem { Text = "16GB or higher", Value = "16GB or higher" },
        };

        public static readonly List<SelectListItem> ConnectionSpeeds = new()
        {
            new() { Text = "0.5Mbps", Value = "0.5Mbps" },
            new() { Text = "1Mbps", Value = "1Mbps" },
            new() { Text = "1.5Mbps", Value = "1.5Mbps" },
            new() { Text = "2Mbps", Value = "2Mbps" },
            new() { Text = "3Mbps", Value = "3Mbps" },
            new() { Text = "5Mbps", Value = "5Mbps" },
            new() { Text = "8Mbps", Value = "8Mbps" },
            new() { Text = "10Mbps", Value = "10Mbps" },
            new() { Text = "15Mbps", Value = "15Mbps" },
            new() { Text = "20Mbps", Value = "20Mbps" },
            new() { Text = "30Mbps", Value = "30Mbps" },
            new() { Text = "Higher than 30Mbps", Value = "Higher than 30Mbps" },
        };
    }
}
