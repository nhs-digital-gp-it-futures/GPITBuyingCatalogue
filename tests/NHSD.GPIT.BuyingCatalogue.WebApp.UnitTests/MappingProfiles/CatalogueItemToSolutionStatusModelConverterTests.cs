using AutoMapper;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class CatalogueItemToSolutionStatusModelConverterTests
    {
        private const string KeyIncomplete = "INCOMPLETE";

        private static readonly object[] ResultSets =
        {
            new object[]{null, KeyIncomplete},
            new object[]{false, KeyIncomplete},
            new object[]{true, "COMPLETE"},
        };

        [Test, CommonAutoData]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsSimpleMappings(
            CatalogueItem catalogueItem)
        {
            var converter = new CatalogueItemToSolutionStatusModelConverter(GetMockMapper().Object);

            var actual = converter.Convert(catalogueItem, default, default);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.CatalogueItemName.Should().Be(catalogueItem.Name);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
            actual.SupplierName.Should().Be(catalogueItem.Supplier.Name);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsAboutSupplierStatus(
            bool complete,
            string expected)
        {
            var mockAboutSupplierModel = new Mock<AboutSupplierModel>();
            mockAboutSupplierModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, AboutSupplierModel>(mockCatalogueItem))
                .Returns(mockAboutSupplierModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, AboutSupplierModel>(mockCatalogueItem));
            mockAboutSupplierModel.VerifyGet(a => a.IsComplete);
            actual.AboutSupplierStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsBrowserBasedStatus(
            bool complete,
            string expected)
        {
            var mockBrowserBasedModel = new Mock<BrowserBasedModel>();
            mockBrowserBasedModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, BrowserBasedModel>(mockCatalogueItem))
                .Returns(mockBrowserBasedModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, BrowserBasedModel>(mockCatalogueItem));
            mockBrowserBasedModel.VerifyGet(a => a.IsComplete);
            actual.BrowserBasedStatus.Should().Be(expected);
        }

        [Test, CommonAutoData]
        public static void Convert_CatalogueItemToSolutionStatusModel_ValidCatalogueItem_SetsClientApplication(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var converter = new CatalogueItemToSolutionStatusModelConverter(GetMockMapper().Object);

            var actual = converter.Convert(catalogueItem, default, default);

            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
        }

        [Test, CommonAutoData]
        public static void Convert_CatalogueItemToSolutionStatusModel_NoClientAppInSolution_ClientApplicationSetToNew(
            CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ClientApplication = null;
            var converter = new CatalogueItemToSolutionStatusModelConverter(GetMockMapper().Object);

            var actual = converter.Convert(catalogueItem, default, default);

            actual.ClientApplication.Should().BeEquivalentTo(new ClientApplication());
        }

        [Test, CommonAutoData]
        public static void Convert_CatalogueItemToSolutionStatusModel_NoSolutionInCatalogueItem_ClientApplicationSetToNew(
            CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var converter = new CatalogueItemToSolutionStatusModelConverter(GetMockMapper().Object);

            var actual = converter.Convert(catalogueItem, default, default);

            actual.ClientApplication.Should().BeEquivalentTo(new ClientApplication());
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsClientApplicationTypeStatus(
            bool complete,
            string expected)
        {
            var mockClientApplicationTypesModel = new Mock<ClientApplicationTypesModel>();
            mockClientApplicationTypesModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, ClientApplicationTypesModel>(mockCatalogueItem))
                .Returns(mockClientApplicationTypesModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, ClientApplicationTypesModel>(mockCatalogueItem));
            mockClientApplicationTypesModel.VerifyGet(a => a.IsComplete);
            actual.ClientApplicationTypeStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsContactDetailsStatus(
            bool complete,
            string expected)
        {
            var mockContactDetailsModel = new Mock<ContactDetailsModel>();
            mockContactDetailsModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, ContactDetailsModel>(mockCatalogueItem))
                .Returns(mockContactDetailsModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, ContactDetailsModel>(mockCatalogueItem));
            mockContactDetailsModel.VerifyGet(a => a.IsComplete);
            actual.ContactDetailsStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsFeaturesStatus(
            bool complete,
            string expected)
        {
            var mockFeaturesModel = new Mock<FeaturesModel>();
            mockFeaturesModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, FeaturesModel>(mockCatalogueItem))
                .Returns(mockFeaturesModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, FeaturesModel>(mockCatalogueItem));
            mockFeaturesModel.VerifyGet(a => a.IsComplete);
            actual.FeaturesStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsHybridStatus(
            bool complete,
            string expected)
        {
            var mockHybridModel = new Mock<HybridModel>();
            mockHybridModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, HybridModel>(mockCatalogueItem))
                .Returns(mockHybridModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, HybridModel>(mockCatalogueItem));
            mockHybridModel.VerifyGet(a => a.IsComplete);
            actual.HybridStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsImplementationTimescalesStatus(
            bool complete,
            string expected)
        {
            var mockImplementationTimescalesModel = new Mock<ImplementationTimescalesModel>();
            mockImplementationTimescalesModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem))
                .Returns(mockImplementationTimescalesModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem));
            mockImplementationTimescalesModel.VerifyGet(a => a.IsComplete);
            actual.ImplementationTimescalesStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsIntegrationsStatus(
            bool complete,
            string expected)
        {
            var mockIntegrationsModel = new Mock<IntegrationsModel>();
            mockIntegrationsModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, IntegrationsModel>(mockCatalogueItem))
                .Returns(mockIntegrationsModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, IntegrationsModel>(mockCatalogueItem));
            mockIntegrationsModel.VerifyGet(a => a.IsComplete);
            actual.IntegrationsStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsOnPremisesStatus(
            bool complete,
            string expected)
        {
            var mockOnPremiseModel = new Mock<OnPremiseModel>();
            mockOnPremiseModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, OnPremiseModel>(mockCatalogueItem))
                .Returns(mockOnPremiseModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, OnPremiseModel>(mockCatalogueItem));
            mockOnPremiseModel.VerifyGet(a => a.IsComplete);
            actual.OnPremisesStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsNativeDesktopStatus(
            bool complete,
            string expected)
        {
            var mockNativeDesktopModel = new Mock<NativeDesktopModel>();
            mockNativeDesktopModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, NativeDesktopModel>(mockCatalogueItem))
                .Returns(mockNativeDesktopModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, NativeDesktopModel>(mockCatalogueItem));
            mockNativeDesktopModel.VerifyGet(a => a.IsComplete);
            actual.NativeDesktopStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsNativeMobileStatus(
            bool complete,
            string expected)
        {
            var mockNativeMobileModel = new Mock<NativeMobileModel>();
            mockNativeMobileModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, NativeMobileModel>(mockCatalogueItem))
                .Returns(mockNativeMobileModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, NativeMobileModel>(mockCatalogueItem));
            mockNativeMobileModel.VerifyGet(a => a.IsComplete);
            actual.NativeMobileStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsPrivateCloudStatus(
            bool complete,
            string expected)
        {
            var mockPrivateCloudModel = new Mock<PrivateCloudModel>();
            mockPrivateCloudModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, PrivateCloudModel>(mockCatalogueItem))
                .Returns(mockPrivateCloudModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, PrivateCloudModel>(mockCatalogueItem));
            mockPrivateCloudModel.VerifyGet(a => a.IsComplete);
            actual.PrivateCloudStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsPublicCloudStatus(
            bool complete,
            string expected)
        {
            var mockPublicCloudModel = new Mock<PublicCloudModel>();
            mockPublicCloudModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, PublicCloudModel>(mockCatalogueItem))
                .Returns(mockPublicCloudModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, PublicCloudModel>(mockCatalogueItem));
            mockPublicCloudModel.VerifyGet(a => a.IsComplete);
            actual.PublicCloudStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsRoadmapStatus(
            bool complete,
            string expected)
        {
            var mockRoadmapModel = new Mock<RoadmapModel>();
            mockRoadmapModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, RoadmapModel>(mockCatalogueItem))
                .Returns(mockRoadmapModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, RoadmapModel>(mockCatalogueItem));
            mockRoadmapModel.VerifyGet(a => a.IsComplete);
            actual.RoadmapStatus.Should().Be(expected);
        }

        [TestCaseSource(nameof(ResultSets))]
        public static void Convert_CatalogueItemToSolutionStatusModel_SetsSolutionDescriptionStatus(
            bool complete,
            string expected)
        {
            var mockSolutionDescriptionModel = new Mock<SolutionDescriptionModel>();
            mockSolutionDescriptionModel.Setup(a => a.IsComplete)
                .Returns(complete);
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMapper = GetMockMapper();
            mockMapper.Setup(m => m.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem))
                .Returns(mockSolutionDescriptionModel.Object);
            var converter = new CatalogueItemToSolutionStatusModelConverter(mockMapper.Object);

            var actual = converter.Convert(mockCatalogueItem, default, default);

            mockMapper.Verify(m => m.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem));
            mockSolutionDescriptionModel.VerifyGet(a => a.IsComplete);
            actual.SolutionDescriptionStatus.Should().Be(expected);
        }

        private static Mock<IMapper> GetMockMapper()
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<CatalogueItem, AboutSupplierModel>(It.IsAny<CatalogueItem>()))
                .Returns(new AboutSupplierModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, BrowserBasedModel>(It.IsAny<CatalogueItem>()))
                .Returns(new BrowserBasedModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, ClientApplicationTypesModel>(It.IsAny<CatalogueItem>()))
                .Returns(new ClientApplicationTypesModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, ContactDetailsModel>(It.IsAny<CatalogueItem>()))
                .Returns(new ContactDetailsModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, FeaturesModel>(It.IsAny<CatalogueItem>()))
                .Returns(new FeaturesModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, HybridModel>(It.IsAny<CatalogueItem>()))
                .Returns(new HybridModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, ImplementationTimescalesModel>(It.IsAny<CatalogueItem>()))
                .Returns(new ImplementationTimescalesModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, IntegrationsModel>(It.IsAny<CatalogueItem>()))
                .Returns(new IntegrationsModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, OnPremiseModel>(It.IsAny<CatalogueItem>()))
                .Returns(new OnPremiseModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, NativeDesktopModel>(It.IsAny<CatalogueItem>()))
                .Returns(new NativeDesktopModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, NativeMobileModel>(It.IsAny<CatalogueItem>()))
                .Returns(new NativeMobileModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, PrivateCloudModel>(It.IsAny<CatalogueItem>()))
                .Returns(new PrivateCloudModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, PublicCloudModel>(It.IsAny<CatalogueItem>()))
                .Returns(new PublicCloudModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, RoadmapModel>(It.IsAny<CatalogueItem>()))
                .Returns(new RoadmapModel());
            mockMapper.Setup(m => m.Map<CatalogueItem, SolutionDescriptionModel>(It.IsAny<CatalogueItem>()))
                .Returns(new SolutionDescriptionModel());

            return mockMapper;
        }
    }
}
