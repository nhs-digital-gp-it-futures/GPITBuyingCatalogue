using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class ActionExecutingContextCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ActionExecutingContext> composer) => composer
                .With(c => c.ActionDescriptor, new ActionDescriptor())
                .With(c => c.HttpContext, new DefaultHttpContext())
                .With(c => c.RouteData, new RouteData())
                .With(c => c.Result, (IActionResult)null);

            fixture.Customize<ActionExecutingContext>(ComposerTransformation);
        }
    }
}
