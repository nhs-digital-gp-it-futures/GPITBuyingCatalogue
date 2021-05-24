using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Errors;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;
using NHSD.GPIT.BuyingCatalogue.Services.CreateBuyer;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.CreateBuyer
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AspNetUserValidatorTests
    {
        [Test]
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

        [TestCaseSource(typeof(TestContextTestCaseData), nameof(TestContextTestCaseData.InvalidFirstNameCases))]
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

        [TestCaseSource(typeof(TestContextTestCaseData), nameof(TestContextTestCaseData.InvalidLastNameCases))]
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

        [TestCaseSource(typeof(TestContextTestCaseData), nameof(TestContextTestCaseData.InvalidPhoneNumberCases))]
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

        [TestCaseSource(typeof(TestContextTestCaseData), nameof(TestContextTestCaseData.InvalidEmailTestCases))]
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

        [Test]
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

        [Test]
        public static void Constructor_NullUserRepository_ThrowsException()
        {
            static void Test()
            {
                _ = new AspNetUserValidator(null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [Test]
        public static void ValidateAsync_NullApplicationUser_ThrowsException()
        {
            var context = ApplicationUserValidatorTestContext.Setup();
            var sut = context.ApplicationUserValidator;

            Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ValidateAsync(null));
        }

        private static class TestContextTestCaseData
        {
            internal static IEnumerable<TestCaseData> InvalidFirstNameCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, new[] { "FirstNameRequired" });
                    yield return new TestCaseData("  ", new[] { "FirstNameRequired" });
                    yield return new TestCaseData(new string('a', 101), new[] { "FirstNameTooLong" });
                }
            }

            internal static IEnumerable<TestCaseData> InvalidLastNameCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, new[] { "LastNameRequired" });
                    yield return new TestCaseData("  ", new[] { "LastNameRequired" });
                    yield return new TestCaseData(new string('a', 101), new[] { "LastNameTooLong" });
                }
            }

            internal static IEnumerable<TestCaseData> InvalidPhoneNumberCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, new[] { "PhoneNumberRequired" });
                    yield return new TestCaseData("  ", new[] { "PhoneNumberRequired" });
                    yield return new TestCaseData(new string('p', 36), new[] { "PhoneNumberTooLong" });
                }
            }

            internal static IEnumerable<TestCaseData> InvalidEmailTestCases
            {
                get
                {
                    yield return new TestCaseData(string.Empty, new[] { "EmailRequired" });
                    yield return new TestCaseData("  ", new[] { "EmailRequired" });
                    yield return new TestCaseData($"a@{new string('b', 255)}", new[] { "EmailTooLong" });
                    yield return new TestCaseData("test", new[] { "EmailInvalidFormat" });
                    yield return new TestCaseData("test@", new[] { "EmailInvalidFormat" });
                    yield return new TestCaseData("@test", new[] { "EmailInvalidFormat" });
                    yield return new TestCaseData("@", new[] { "EmailInvalidFormat" });
                    yield return new TestCaseData("bobsmith@test@com", new[] { "EmailInvalidFormat" });
                }
            }
        }

        private sealed class ApplicationUserValidatorTestContext
        {
            private ApplicationUserValidatorTestContext()
            {
                UsersRepositoryMock = new Mock<IDbRepository<AspNetUser, UsersDbContext>>();
                UsersRepositoryMock
                    .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AspNetUser, bool>>>()))
                    .ReturnsAsync(() => new[] { ApplicationUserByEmail });

                ApplicationUserValidator = new AspNetUserValidator(UsersRepositoryMock.Object);
            }

            internal AspNetUserValidator ApplicationUserValidator { get; }

            internal AspNetUser ApplicationUserByEmail { get; set; }

            private Mock<IDbRepository<AspNetUser, UsersDbContext>> UsersRepositoryMock { get; }

            public static ApplicationUserValidatorTestContext Setup()
            {
                return new();
            }
        }
    }
}
