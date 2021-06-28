using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Identity
{
    public static class PasswordResetTokenTests
    {
        [Fact]
        public static void Constructor_String_ApplicationUser_InitializesExpectedMembers()
        {
            const string expectedToken = "TokenToken";
            var expectedUser = AspNetUserBuilder.Create().Build();

            var token = new PasswordResetToken(expectedToken, expectedUser);

            token.Token.Should().Be(expectedToken);
            token.User.Should().Be(expectedUser);
        }
    }
}
