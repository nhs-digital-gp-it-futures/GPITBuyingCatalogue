using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AspNetUserExtensionsTests
    {
        [Test]
        public static void AspNetUserExtention_FormatsDisplayName()
        {
            var user = new AspNetUser { FirstName = "Bob", LastName = "Smith" };
            
            Assert.AreEqual("Bob Smith", user.GetDisplayName());
        }      
    }
}
