using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Errors;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;
using NHSD.GPIT.BuyingCatalogue.Services.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.CreateBuyer
{
    public static class AspNetUserValidatorTests
    {
        [Fact]
        public static async Task ValidateAsync_ValidApplicationUser_ReturnsSuccess()
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = AspNetUserBuilder
                .Create()
                .Build();

            var actual = await sut.ValidateAsync(user);

            actual.Should().Be(Result.Success());
        }

        [Theory]
        [MemberData(nameof(TestContextTestCaseData.InvalidFirstNameCases), MemberType = typeof(TestContextTestCaseData))]
        public static async Task ValidateAsync_WithFirstName_ReturnsFailure(string input, params string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = AspNetUserBuilder
                .Create()
                .WithFirstName(input)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, nameof(AspNetUser.FirstName))));
            actual.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(TestContextTestCaseData.InvalidLastNameCases), MemberType = typeof(TestContextTestCaseData))]
        public static async Task ValidateAsync_WithLastName_ReturnsFailure(string input, params string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = AspNetUserBuilder
                .Create()
                .WithLastName(input)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, nameof(AspNetUser.LastName))));
            actual.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(TestContextTestCaseData.InvalidPhoneNumberCases), MemberType = typeof(TestContextTestCaseData))]
        public static async Task ValidateAsync_WithPhoneNumber_ReturnsFailure(string input, params string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = AspNetUserBuilder
                .Create()
                .WithPhoneNumber(input)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, nameof(AspNetUser.PhoneNumber))));
            actual.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(TestContextTestCaseData.InvalidEmailTestCases), MemberType = typeof(TestContextTestCaseData))]
        public static async Task ValidateAsync_WithEmailAddress_ReturnsFailure(string input, string[] errorMessageIds)
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            var user = AspNetUserBuilder
                .Create()
                .WithEmailAddress(input)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(errorMessageIds.Select(id => new ErrorDetails(id, "EmailAddress")));
            actual.Should().Be(expected);
        }

        [Fact]
        public static async Task ValidateAsync_DuplicateEmailAddress_ReturnsFailure()
        {
            const string duplicateEmailAddress = "duplicate@email.com";

            var context = ApplicationUserValidatorTestContext.Setup();
            context.ApplicationUserByEmail = AspNetUserBuilder
                .Create()
                .WithEmailAddress(duplicateEmailAddress)
                .Build();

            var sut = context.ApplicationUserValidator;

            var user = AspNetUserBuilder
                .Create()
                .WithEmailAddress(duplicateEmailAddress)
                .Build();

            var actual = await sut.ValidateAsync(user);

            var expected = Result.Failure(new List<ErrorDetails> { new("EmailAlreadyExists", "EmailAddress") });
            actual.Should().Be(expected);
        }

        [Fact]
        public static void Constructor_NullUserRepository_ThrowsException()
        {
            static void Test()
            {
                _ = new AspNetUserValidator(null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Fact]
        public static void ValidateAsync_NullApplicationUser_ThrowsException()
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ValidateAsync(null));
        }

        private static class TestContextTestCaseData
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
        }

        private sealed class ApplicationUserValidatorTestContext
        {
            private ApplicationUserValidatorTestContext()
            {
                UsersRepositoryMock = new Mock<IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext>>();
                UsersRepositoryMock
                    .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                    .ReturnsAsync(() => new[] { ApplicationUserByEmail });

                ApplicationUserValidator = new AspNetUserValidator(UsersRepositoryMock.Object);
            }

            internal AspNetUserValidator ApplicationUserValidator { get; }

            internal AspNetUser ApplicationUserByEmail { get; set; }

            private Mock<IDbRepository<AspNetUser, GPITBuyingCatalogueDbContext>> UsersRepositoryMock { get; }

            public static ApplicationUserValidatorTestContext Setup()
            {
                return new();
            }
        }
    }
}
