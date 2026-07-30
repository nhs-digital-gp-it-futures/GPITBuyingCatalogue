using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;
using static Microsoft.Playwright.Assertions;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Tests.Login;

public class LoginTests : BaseTest
{
    private IFrameLocator RecaptchaFrame => Page.FrameLocator("iframe[title='reCAPTCHA']");
    private ILocator RecaptchaCheckbox => RecaptchaFrame.Locator("#recaptcha-anchor");

    public LoginTests(TestServerFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    [Fact]
    [Trait("Category", Categories.Smoke)]
    public async Task UserCanLoginSuccessfully()
    {
        var data = new OrderTestDataBuilder()
            .WithBaseUrl(Fixture.BaseUrl)
            .Build();

        await orderPages.LoginAsync();
    }

    [Fact]
    public async Task AdminAddSolution()
    {
        //await orderPages.LoginAsync();
        await Page.GotoAsync(Fixture.BaseUrl);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Log in" }).ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("login", Page.Url.ToLower());

        await Page.GetByLabel("Email").FillAsync("bobsmith@email.com");
        await Page.GetByLabel("Password").FillAsync("Pass123$");
        await CompleteRecaptchaAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Buying Catalogue admin" }).IsVisibleAsync());

        await Page.GetByRole(AriaRole.Link, new() { Name = "Manage catalogue solutions" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Manage catalogue solutions" }).IsVisibleAsync());

        //Add solution
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add a solution" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Add a solution" }).IsVisibleAsync());

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Solution name" }).FillAsync($"Test {Random.Shared.Next(10, 100)}");
        await Page.GetByLabel("Supplier name").SelectOptionAsync(new[] { "10000" });
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Tech Innovation Framework" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.GetByText("Explain what your catalogue solution is for")).ToBeVisibleAsync();

        //Description
        await Page.GetByRole(AriaRole.Row, new() { Name = "Description" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Summary" }).FillAsync("safasdf");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Full description (optional)" }).FillAsync("asfdasfs");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.GetByText("Explain what your catalogue solution is for")).ToBeVisibleAsync();

        //Features
        await Page.GetByRole(AriaRole.Row, new() { Name = "Features" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Feature 1 (optional)" }).FillAsync("asdfsafa");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Feature 2 (optional)" }).FillAsync("sdfas");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.GetByText("Explain what your catalogue solution is for")).ToBeVisibleAsync();

        //Application type
        await Page.GetByRole(AriaRole.Row, new() { Name = "Application type" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Application type" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add an application type" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Add an application type" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Desktop" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Desktop application" }).IsVisibleAsync());

        //Application type - Supported operating systems
        await Page.GetByRole(AriaRole.Row, new() { Name = "Supported operating systems" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Desktop application – supported operating systems" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Supported operating systems" }).FillAsync("Windows");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Application type - Connectivity
        await Page.GetByRole(AriaRole.Row, new() { Name = "Connectivity" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Desktop application ­– connectivity" }).IsVisibleAsync());
        await Page.GetByLabel("Connection speed").SelectOptionAsync(new[] { "Higher than 30Mbps" });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Desktop application" }).IsVisibleAsync());

        //Application type - Memory, storage, processing and resolution
        await Page.GetByRole(AriaRole.Row, new() { Name = "Memory, storage, processing and resolution" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Desktop application – memory, storage, processing and resolution" }).IsVisibleAsync());
        await Page.GetByLabel("Memory size").SelectOptionAsync(new[] { "256MB" });
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Storage space" }).FillAsync("gdagasd");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Processing power" }).FillAsync("agsdgad");
        await Page.GetByLabel("Screen resolution and aspect").SelectOptionAsync(new[] { "16:9 - 3840 x 2160" });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Desktop application" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Application type" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.GetByText("Explain what your catalogue solution is for")).ToBeVisibleAsync();

        //Hosting type
        await Page.GetByRole(AriaRole.Row, new() { Name = "Hosting type" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Hosting type" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add a hosting type" }).ClickAsync();
        await Page.GetByText("On premise").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Summary" }).FillAsync("fsas");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Data centre model" }).FillAsync("dfadf");
        await Page.GetByText("Yes, devices must be").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Hosting type" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.GetByText("Explain what your catalogue solution is for")).ToBeVisibleAsync();


        //List Price
        await Page.GetByRole(AriaRole.Row, new() { Name = "List price" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "List price" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add a list price" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "List price type" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Flat price" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Add a flat list price" }).IsVisibleAsync());
        await Page.GetByText("Per patient per year").ClickAsync();
        await Page.GetByText("Single fixed").ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Price" }).FillAsync("1.5");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Unit", Exact = true }).FillAsync("per patient");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Units" }).FillAsync("patients");
        await Page.GetByText("Publish").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "List price" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.GetByText("Explain what your catalogue solution is for")).ToBeVisibleAsync();

        //Capabilities and Epics
        await Page.GetByRole(AriaRole.Row, new() { Name = "Capabilities and Epics" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Capabilities and Epics" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "(C5) Appointments Management" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.GetByText("Explain what your catalogue solution is for")).ToBeVisibleAsync();

        //Supplier details
        await Page.GetByRole(AriaRole.Row, new() { Name = "Supplier details" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Supplier details" }).IsVisibleAsync());
        await Page.GetByText("Jonas Chan").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.GetByText("Explain what your catalogue solution is for")).ToBeVisibleAsync();

        //Service Level Agreement
        await Page.GetByRole(AriaRole.Row, new() { Name = "Service Level Agreement" }).GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution type" }).IsVisibleAsync());
        await Page.GetByText("Type 2 catalogue solution").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Service Level Agreement" }).IsVisibleAsync());

        //Service Level Agreement - Service availability times
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add availability times" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Service availability times" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Type of support" }).FillAsync("afsda");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "From" }).FillAsync("09:00");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Until" }).FillAsync("18:00");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Monday" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Tuesday" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Wednesday" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Thursday" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Friday" }).CheckAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes, include Bank Holidays" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Service Level Agreement" }).IsVisibleAsync());

        //Service Level Agreement - Support contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add contact details" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Support contact details" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contact channel" }).FillAsync("web chat");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contact information" }).FillAsync("01274231312");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "From" }).FillAsync("09:00");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Until" }).FillAsync("18:00");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Service Level Agreement" }).IsVisibleAsync());

        //Service Level Agreement - Service levels
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add service levels" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Service levels" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Type of service" }).FillAsync("afsadfsaas");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Service level", Exact = true }).FillAsync("dfaasdfsad");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Service level", Exact = true }).FillAsync("dfaasdfsads");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "How the service levels are" }).FillAsync("dfasfsdfa");
        await Page.GetByText("No", new() { Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Service Level Agreement" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.GetByText("Explain what your catalogue solution is for")).ToBeVisibleAsync();

        //Service Level Agreement
        await Page.GetByText("Publish", new() { Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Search by supplier or" }).ClickAsync();
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Search by supplier or" }).FillAsync("test");
    }



    private static async Task SelectAndConfirmSupplier(IPage Page, string supplierName)
    {
        var supplierInput = Page.Locator("input[role='combobox']:visible");
        var suggestionsList = Page.Locator("ul[role='listbox']:visible");
        var saveButton = Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" });

        await supplierInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await supplierInput.ClickAsync();
        await supplierInput.FillAsync(string.Empty);
        await supplierInput.PressSequentiallyAsync(supplierName);

        await suggestionsList.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await suggestionsList.GetByText(supplierName, new() { Exact = true }).ClickAsync();

        await saveButton.ClickAsync();
    }

    private async Task CompleteRecaptchaAsync()
    {
        await RecaptchaCheckbox.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await RecaptchaCheckbox.ClickAsync();

        // Wait for the checkbox to be checked before continuing
        await Page.FrameLocator("iframe[title='reCAPTCHA']")
            .Locator("#recaptcha-anchor[aria-checked='true']")
            .WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
    }

    private async Task DeleteEmailDomainAsync(string domain)
    {
        await Page.GetByRole(AriaRole.Row, new() { Name = domain })
            .GetByRole(AriaRole.Link, new() { Name = "Delete" })
            .ClickAsync();
    }

    private ILocator OrganisationInput => Page.Locator("input[role='combobox']:visible");
    private ILocator SuggestionsList => Page.Locator("ul[role='listbox']:visible");

    private async Task SelectOrganisationAsync(string organisationName)
    {
        await OrganisationInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await OrganisationInput.ClickAsync();
        await OrganisationInput.FillAsync(string.Empty);
        await OrganisationInput.PressSequentiallyAsync(organisationName);

        await SuggestionsList.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await SuggestionsList.GetByText(organisationName, new() { Exact = true }).ClickAsync();
    }
}
