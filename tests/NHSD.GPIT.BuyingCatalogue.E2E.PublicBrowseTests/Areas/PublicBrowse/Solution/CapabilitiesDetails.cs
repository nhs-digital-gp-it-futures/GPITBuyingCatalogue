﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
   public  class CapabilitiesDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public CapabilitiesDetails(LocalWebApplicationFactory factory) : base(factory, "solutions/futures/99999-001/capabilities")
        {
        }

        [Fact]
        public async Task CapabilitiesDetails_VerifyCapabilities()
        {
            {
                using var context = GetBCContext();
                var capabilitiesInfo = (await context.Solutions.Include(s=> s.SolutionCapabilities).ThenInclude(s=>s.Capability).SingleAsync(s => s.Id == "99999-001")).SolutionCapabilities.Select(s=> s.Capability);
                var capabilitiesList = PublicBrowsePages.SolutionAction.GetCapabilitiesContent().ToArray()[0];

                var capabilitiesTitle = capabilitiesInfo.Select(c => c.Name.Trim());
                foreach (var name in capabilitiesTitle)
                {
                    capabilitiesList.Should().Contain(name);
                }

                var capabilitiesDescription = capabilitiesInfo.Select(b => b.Description.Trim());
                foreach (var description in capabilitiesDescription)
                {
                    capabilitiesList.Should().Contain(description);
                }
            }
        }
    }
}
