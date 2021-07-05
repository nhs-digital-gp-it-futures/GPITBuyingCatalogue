using System.IO;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class DocumentControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(IdentityService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async void InvalidDocument_ReturnsNotFoundView(
            [Frozen] Mock<IDocumentService> mockDocumentService,
            string documentName)
        {
            mockDocumentService.Setup(d => d.DownloadDocumentAsync(documentName))
                .ReturnsAsync((FileStreamResult)null);

            var controller = new DocumentController(mockDocumentService.Object);

            var result = await controller.GetDocument(documentName) as ViewResult;

            result.Should().NotBeNull();
            result.ViewName.Should().Be("NotFound");
        }

        [Theory]
        [CommonAutoData]
        public static async void ValidDocumentName_ReturnsFileStream(
            [Frozen] Mock<IDocumentService> mockDocumentService,
            string documentName)
        {
            const string expectedContentType = "test/content-type";
            await using var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes("Hello world!"));
            var fileStreamResult = new FileStreamResult(expectedStream, expectedContentType);

            mockDocumentService.Setup(d => d.DownloadDocumentAsync(documentName))
                .ReturnsAsync(fileStreamResult);

            var controller = new DocumentController(mockDocumentService.Object);

            var result = await controller.GetDocument(documentName) as FileStreamResult;

            result.Should().NotBeNull();
            result.ContentType.Should().Be(expectedContentType);
            result.FileStream.IsSameOrEqualTo(expectedStream);
        }
    }
}
