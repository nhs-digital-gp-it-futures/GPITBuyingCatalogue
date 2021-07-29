﻿using AutoFixture;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class HostingTypeSectionModelCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<HostingTypeSectionModel>(c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery()))
                .Without(m => m.SolutionId)
                .Without(m => m.SolutionName));
        }
    }
}
