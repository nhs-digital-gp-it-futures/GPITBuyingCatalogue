using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Errors;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;
using NHSD.GPIT.BuyingCatalogue.Services.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.CreateBuyer
{
    public static class AspNetUserValidatorTests
    {
        public static IEnumerable<object[]> InvalidFirstNameCases
        {
            get
            {
                yield return new object[] { string.Empty, new[] { "FirstNameRequired" } };
                yield return new object[] { "  ", new[] { "FirstNameRequired" } };
                yield return new object[] { new string('a', 101), new[] { "FirstNameTooLong" } };
            }
        }

        public static IEnumerable<object[]> InvalidLastNameCases
        {
            get
            {
                yield return new object[] { string.Empty, new[] { "LastNameRequired" } };
                yield return new object[] { "  ", new[] { "LastNameRequired" } };
                yield return new object[] { new string('a', 101), new[] { "LastNameTooLong" } };
            }
        }

        public static IEnumerable<object[]> InvalidPhoneNumberCases
        {
            get
            {
                yield return new object[] { string.Empty, new[] { "PhoneNumberRequired" } };
                yield return new object[] { "  ", new[] { "PhoneNumberRequired" } };
                yield return new object[] { new string('p', 36), new[] { "PhoneNumberTooLong" } };
            }
        }

        public static IEnumerable<object[]> InvalidEmailTestCases
        {
            get
            {
                yield return new object[] { string.Empty, new[] { "EmailRequired" } };
                yield return new object[] { "  ", new[] { "EmailRequired" } };
                yield return new object[] { $"a@{new string('b', 255)}", new[] { "EmailTooLong" } };
                yield return new object[] { "test", new[] { "EmailInvalidFormat" } };
                yield return new object[] { "test@", new[] { "EmailInvalidFormat" } };
                yield return new object[] { "@test", new[] { "EmailInvalidFormat" } };
                yield return new object[] { "@", new[] { "EmailInvalidFormat" } };
                yield return new object[] { "bobsmith@test@com", new[] { "EmailInvalidFormat" } };
            }
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CreateBuyerService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ValidateAsync_ValidApplicationUser_ReturnsSuccess(AspNetUserValidator validator)
        {
            var user = AspNetUserBuilder
                .Create()
                .Build();

            var actual = await validator.ValidateAsync(user);

            actual.Should().Be(Result.Success());
        }

        [Theory]
        [InMemoryDbMemberAutoData(nameof(InvalidFirstNameCases))]
        public static async Task ValidateAsync_WithFirstName_ReturnsFailure(
            string input,
            string[] errorMessageIds,
            AspNetUserValidator validator)
        {
            var user = AspNetUserBuilder
                .Create()
                .WithFirstName(input)
                .Build();

            var actual = await validator.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, nameof(AspNetUser.FirstName))));
            actual.Should().Be(expected);
        }

        [Theory]
        [InMemoryDbMemberAutoData(nameof(InvalidLastNameCases))]
        public static async Task ValidateAsync_WithLastName_ReturnsFailure(
            string input,
            string[] errorMessageIds,
            AspNetUserValidator validator)
        {
            var user = AspNetUserBuilder
                .Create()
                .WithLastName(input)
                .Build();

            var actual = await validator.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, nameof(AspNetUser.LastName))));
            actual.Should().Be(expected);
        }

        [Theory]
        [InMemoryDbMemberAutoData(nameof(InvalidPhoneNumberCases))]
        public static async Task ValidateAsync_WithPhoneNumber_ReturnsFailure(
            string input,
            string[] errorMessageIds,
            AspNetUserValidator validator)
        {
            var user = AspNetUserBuilder
                .Create()
                .WithPhoneNumber(input)
                .Build();

            var actual = await validator.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, nameof(AspNetUser.PhoneNumber))));
            actual.Should().Be(expected);
        }

        [Theory]
        [InMemoryDbMemberAutoData(nameof(InvalidEmailTestCases))]
        public static async Task ValidateAsync_WithEmailAddress_ReturnsFailure(
            string input,
            string[] errorMessageIds,
            AspNetUserValidator validator)
        {
            var user = AspNetUserBuilder
                .Create()
                .WithEmailAddress(input)
                .Build();

            var actual = await validator.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, "EmailAddress")));
            actual.Should().Be(expected);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ValidateAsync_DuplicateEmailAddress_ReturnsFailure(
            [Frozen] BuyingCatalogueDbContext context,
            AspNetUser existingUser,
            AspNetUserValidator validator)
        {
            const string duplicateEmailAddress = "duplicate@email.com";

            existingUser.NormalizedEmail = duplicateEmailAddress.ToUpperInvariant();
            existingUser.Email = duplicateEmailAddress;
            context.AspNetUsers.Add(existingUser);
            await context.SaveChangesAsync();

            var user = AspNetUserBuilder
                .Create()
                .WithEmailAddress(duplicateEmailAddress)
                .Build();

            var actual = await validator.ValidateAsync(user);

            var expected = Result.Failure(new List<ErrorDetails> { new("EmailAlreadyExists", "EmailAddress") });
            actual.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public static Task ValidateAsync_NullApplicationUser_ThrowsException(
            AspNetUserValidator validator)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => validator.ValidateAsync(null));
        }
    }
}
