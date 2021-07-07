using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
     public class AdditionalServicesDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AdditionalServicesDetails(LocalWebApplicationFactory factory) : base(factory, "solutions/futures/100000-001/additional-services")
        {
        }
    }
}
