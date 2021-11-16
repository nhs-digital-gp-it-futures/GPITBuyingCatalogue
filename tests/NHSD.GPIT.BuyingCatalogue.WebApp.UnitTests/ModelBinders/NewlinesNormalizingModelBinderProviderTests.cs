using System;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ModelBinders
{
    public static class NewlinesNormalizingModelBinderProviderTests
    {
        [Fact]
        public static void GetBinder_NullContext_ThrowsException()
        {
            var provider = new NewlinesNormalizingModelBinderProvider();

            Assert.Throws<ArgumentNullException>(() => provider.GetBinder(null));
        }
    }
}
