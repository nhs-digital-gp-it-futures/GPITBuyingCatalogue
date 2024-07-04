using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

internal sealed class RequestedFiltersCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<RequestedFilters> composer) => composer
            .FromFactory(new RequestedFiltersSpecimenBuilder());

        fixture.Customize<RequestedFilters>(ComposerTransformation);
    }

    private sealed class RequestedFiltersSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!(request as Type == typeof(RequestedFilters)))
                return new NoSpecimen();

            var selectedCapabilitiesAndEpics = new Dictionary<int, string[]> { { 1, ["E00001"] }, };
            var selectedIntegrations = new Dictionary<SupportedIntegrations, int[]>
            {
                { SupportedIntegrations.Im1, [1, 2] }, { SupportedIntegrations.GpConnect, [3, 4] },
            };

            return new RequestedFilters(
                selectedCapabilitiesAndEpics.ToFilterString(),
                selectedIntegrations.ToFilterString(),
                context.Create<string>(),
                context.Create<string>(),
                context.Create<string>(),
                context.Create<string>(),
                context.Create<string>());
        }
    }
}
