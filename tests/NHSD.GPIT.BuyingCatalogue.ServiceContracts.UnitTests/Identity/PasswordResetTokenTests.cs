using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Identity
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class PasswordResetTokenTests
    {
        [Test]
        public static void Constructor_String_ApplicationUser_InitializesExpectedMembers()
        {
            const string expectedToken = "TokenToken";
            var expectedUser = ApplicationUserBuilder.Create().Build();

            var token = new PasswordResetToken(expectedToken, expectedUser);

            token.Token.Should().Be(expectedToken);
            token.User.Should().Be(expectedUser);
        }
    }
}
