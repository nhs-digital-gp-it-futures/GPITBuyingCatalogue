using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class ActionExecutedContextCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ActionExecutedContext> composer) => composer
                .With(c => c.ActionDescriptor, new ActionDescriptor())
                .With(c => c.HttpContext, new DefaultHttpContext())
                .With(c => c.RouteData, new RouteData());

            fixture.Customize<ActionExecutedContext>(ComposerTransformation);
        }
    }
}
