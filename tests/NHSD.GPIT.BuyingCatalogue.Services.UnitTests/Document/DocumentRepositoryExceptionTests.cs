using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Services.Document;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Document
{
    public static class DocumentRepositoryExceptionTests
    {
        [Fact]
        public static void Constructor_Exception_Int_InitializesCorrectly()
        {
            const string message = "This is a message.";
            const int statusCode = 404;

            var innerException = new InvalidOperationException(message);
            var repositoryException = new DocumentRepositoryException(innerException, statusCode);

            repositoryException.HttpStatusCode.Should().Be(statusCode);
            repositoryException.InnerException.Should().Be(innerException);
            repositoryException.Message.Should().Be(message);
        }

        [Fact]
        public static void Constructor_InitializesCorrectly()
        {
            var repositoryException = new DocumentRepositoryException();

            repositoryException.HttpStatusCode.Should().Be(0);
            repositoryException.InnerException.Should().BeNull();
            repositoryException.Message.Should().Be(DocumentRepositoryException.DefaultMessage);
        }

        [Fact]
        public static void Constructor_String_Exception_InitializesCorrectly()
        {
            const string message = "This is a message.";

            var innerException = new InvalidOperationException();
            var repositoryException = new DocumentRepositoryException(message, innerException);

            repositoryException.HttpStatusCode.Should().Be(0);
            repositoryException.InnerException.Should().Be(innerException);
            repositoryException.Message.Should().Be(message);
        }

        [Fact]
        public static void Constructor_String_InitializesCorrectly()
        {
            const string message = "This is a message.";

            var repositoryException = new DocumentRepositoryException(message);

            repositoryException.HttpStatusCode.Should().Be(0);
            repositoryException.InnerException.Should().BeNull();
            repositoryException.Message.Should().Be(message);
        }
    }
}
