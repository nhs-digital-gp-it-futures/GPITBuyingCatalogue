using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.EmailDomainManagement;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.EmailDomainManagement;

public class DeleteEmailDomainModelTests
{
    [Theory]
    [MockAutoData]
    public static void WithWildcards_SetsAdviceText(
        EmailDomain emailDomain)
    {
        emailDomain.Domain = "@*.nhs.net";

        var model = new DeleteEmailDomainModel(emailDomain);

        model.AdviceText.Should()
            .Be(
                $"The email domain {model.EmailDomain} and any subdomains related to it will be deleted from the allow list.");
    }

    [Theory]
    [MockAutoData]
    public static void WithoutWildcards_SetsAdviceText(
        EmailDomain emailDomain)
    {
        emailDomain.Domain = "@nhs.net";

        var model = new DeleteEmailDomainModel(emailDomain);

        model.AdviceText.Should()
            .Be(
                $"The email domain {model.EmailDomain} will be deleted from the allow list.");
    }
}
