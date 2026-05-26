using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Tests.Login;

public class LoginTests : BaseTest
{
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
    public async Task AssociatedServiceOnlyOrderSomeThingElse()
    {
        await Page.GotoAsync(Fixture.BaseUrl);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Log in" }).ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("login", Page.Url.ToLower());

        await Page.GetByLabel("Email").FillAsync("suesmith@email.com");
        await Page.GetByLabel("Password").FillAsync("Pass123$");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's dashboard" }).IsVisibleAsync());

        await Page.Locator("li.nhsuk-card-group__item").Filter(new() { HasText = "orders" })
                  .GetByRole(AriaRole.Link, new() { Name = "View orders" })
                  .ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's orders" }).IsVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Create new order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Associated service only" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();


        await Page.GetByRole(AriaRole.Heading, new() { Name = "What type of associated service do you want to order?" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Something else" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Which contracting vehicle are you buying from?" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Tech Innovation" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).FillAsync("Test Order");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Primary contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Primary contact details" }).ClickAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "First name" }).FillAsync("First name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" }).FillAsync("Last name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Telephone number" }).FillAsync("0121233654445");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).FillAsync("test@test.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();


        //Supplier information and contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Supplier information and" }).ClickAsync();

        await SelectAndConfirmSupplier(Page, "EMIS Health");
        await Page.GetByLabel("Yes").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm supplier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Supplier contact details" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Timescales for call-off agreement
        await Page.GetByRole(AriaRole.Link, new() { Name = "Timescales for call-off agreement" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Timescales for call-off agreement" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month", Exact = true }).FillAsync("04");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement initial period (in months)?" }).FillAsync("6");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement duration (in months)?" }).FillAsync("12");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 2
        //Service Recipients
        await Page.GetByRole(AriaRole.Link, new() { Name = "service recipients" }).ClickAsync();
        await Page.GetByLabel("Add service recipients manually").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Select sublocations for this order" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "02T" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BANKFIELD SURGERY" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm service recipients" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Solution and services
        await Page.GetByRole(AriaRole.Link, new() { Name = "Solutions and services" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Which catalogue solution does the service help implement?" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Emis Web GP" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Add associated services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Engineering" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Associated service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of associated service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BANKFIELD SURGERY" }).GetByRole(AriaRole.Textbox).FillAsync("2");
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BEECHWOOD MEDICAL CENTRE" }).GetByRole(AriaRole.Textbox).FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of associated service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Edit solutions and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Planned delivery date
        await Page.GetByRole(AriaRole.Link, new() { Name = "Planned delivery dates" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Planned delivery date" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month" }).FillAsync("06");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review planned delivery dates" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Select funding sources
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select funding sources" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "Engineering" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Local funding" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();


        //Step 3
        //Associated service milestone
        await Page.GetByRole(AriaRole.Link, new() { Name = "Associated service milestones" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Associated service milestones and payment triggers" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Associated service requirements
        await Page.GetByRole(AriaRole.Link, new() { Name = "Associated service requirements" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Associated service requirements" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Data processing information
        await Page.GetByRole(AriaRole.Link, new() { Name = "Data processing information" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Data processing information" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Declaration
        await Page.GetByRole(AriaRole.Link, new() { Name = "Declaration" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Declaration" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "I understand and agree to the" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 4
        //Review and complete order
        await Page.GetByRole(AriaRole.Link, new() { Name = "Review and complete order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review and complete order" }).IsVisibleAsync();
    }

    [Fact]
    public async Task AssociatedServiceOnlyOrderMerger()
    {
        await Page.GotoAsync(Fixture.BaseUrl);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Log in" }).ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("login", Page.Url.ToLower());

        await Page.GetByLabel("Email").FillAsync("suesmith@email.com");
        await Page.GetByLabel("Password").FillAsync("Pass123$");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's dashboard" }).IsVisibleAsync());

        await Page.Locator("li.nhsuk-card-group__item").Filter(new() { HasText = "orders" })
                  .GetByRole(AriaRole.Link, new() { Name = "View orders" })
                  .ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's orders" }).IsVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Create new order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Associated service only" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();


        await Page.GetByRole(AriaRole.Heading, new() { Name = "What type of associated service do you want to order?" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Merger" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Which contracting vehicle are you buying from?" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Tech Innovation" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).FillAsync("Test Order");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Primary contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Primary contact details" }).ClickAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "First name" }).FillAsync("First name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" }).FillAsync("Last name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Telephone number" }).FillAsync("0121233654445");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).FillAsync("test@test.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();


        //Supplier information and contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Supplier information and" }).ClickAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "EMIS Health" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByLabel("Yes").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm supplier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Supplier contact details" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Timescales for call-off agreement
        await Page.GetByRole(AriaRole.Link, new() { Name = "Timescales for call-off agreement" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Timescales for call-off agreement" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month", Exact = true }).FillAsync("04");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement initial period (in months)?" }).FillAsync("6");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement duration (in months)?" }).FillAsync("12");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 2
        //Service Recipients
        await Page.GetByRole(AriaRole.Link, new() { Name = "service recipients" }).ClickAsync();
        await Page.GetByLabel("Add service recipients manually").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Select sublocations for this order" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "02T" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BANKFIELD SURGERY" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BRIG ROYD SURGERY" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Service recipient to be retained" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "BANKFIELD SURGERY" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm service recipients" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Solution and services
        await Page.GetByRole(AriaRole.Link, new() { Name = "Solutions and services" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Which catalogue solution does the service help implement?" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Video Consult" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Edit associated service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Associated service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Edit associated service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Planned delivery date
        await Page.GetByRole(AriaRole.Link, new() { Name = "Planned delivery dates" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Planned delivery date" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month" }).FillAsync("06");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review planned delivery dates" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Select funding sources
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select funding sources" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "Merger" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Local funding" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();


        //Step 3
        //Associated service milestone
        await Page.GetByRole(AriaRole.Link, new() { Name = "Associated service milestones" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Associated service milestones and payment triggers" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Associated service requirements
        await Page.GetByRole(AriaRole.Link, new() { Name = "Associated service requirements" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Associated service requirements" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Data processing information
        await Page.GetByRole(AriaRole.Link, new() { Name = "Data processing information" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Data processing information" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Declaration
        await Page.GetByRole(AriaRole.Link, new() { Name = "Declaration" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Declaration" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "I understand and agree to the" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 4
        //Review and complete order
        await Page.GetByRole(AriaRole.Link, new() { Name = "Review and complete order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review and complete order" }).IsVisibleAsync();
    }

    [Fact]
    public async Task OrderOne()
    {
        await Page.GotoAsync(Fixture.BaseUrl);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Log in" }).ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("login", Page.Url.ToLower());

        await Page.GetByLabel("Email").FillAsync("suesmith@email.com");
        await Page.GetByLabel("Password").FillAsync("Pass123$");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's dashboard" }).IsVisibleAsync());

        await Page.Locator("li.nhsuk-card-group__item").Filter(new() { HasText = "orders" })
                  .GetByRole(AriaRole.Link, new() { Name = "View orders" })
                  .ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's orders" }).IsVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Create new order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start order" }).ClickAsync();
        await Page.GetByText("Catalogue solution and other").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Tech Innovation" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).FillAsync("Test Order");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Primary contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Primary contact details" }).ClickAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "First name" }).FillAsync("First name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" }).FillAsync("Last name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Telephone number" }).FillAsync("0121233654445");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).FillAsync("test@test.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();


        //Supplier information and contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Supplier information and" }).ClickAsync();

        await SelectAndConfirmSupplier(Page, "AccuRx Limited");
        await Page.GetByLabel("Yes").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm supplier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Supplier contact details" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Timescales for call-off agreement
        await Page.GetByRole(AriaRole.Link, new() { Name = "Timescales for call-off agreement" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Timescales for call-off agreement" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month", Exact = true }).FillAsync("04");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement initial period (in months)?" }).FillAsync("6");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement duration (in months)?" }).FillAsync("12");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 2
        //Service Recipients
        await Page.GetByRole(AriaRole.Link, new() { Name = "service recipients" }).ClickAsync();
        await Page.GetByLabel("Add service recipients manually").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Select sublocations for this order" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "02T" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BANKFIELD SURGERY" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm service recipients" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Solution and services
        await Page.GetByRole(AriaRole.Link, new() { Name = "Solutions and services" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solutions" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "AccuRx" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Catalogue solution" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Row, new() { Name = "BANKFIELD SURGERY B84016" }).GetByLabel("Patient total").FillAsync("2");
        await Page.GetByRole(AriaRole.Row, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).GetByLabel("Patient total").FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Edit solutions and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Planned delivery date
        await Page.GetByRole(AriaRole.Link, new() { Name = "Planned delivery dates" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Planned delivery date" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month" }).FillAsync("06");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review planned delivery dates" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();


        //Select funding sources
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select funding sources" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "AccuRx" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Local funding" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 3
        //Implementation and payment
        await Page.GetByRole(AriaRole.Link, new() { Name = "Implementation milestones and payment triggers" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Implementation milestones and payment triggers" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Data processing information
        await Page.GetByRole(AriaRole.Link, new() { Name = "Data processing information" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Data processing information" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Declaration
        await Page.GetByRole(AriaRole.Link, new() { Name = "Declaration" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Declaration" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "I understand and agree to the" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 4
        //Review and complete order
        await Page.GetByRole(AriaRole.Link, new() { Name = "Review and complete order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review and complete order" }).IsVisibleAsync();
    }

    [Fact]
    public async Task OrderWithCatalogueSolutionAndAssociatedService()
    {
        await Page.GotoAsync(Fixture.BaseUrl);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Log in" }).ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("login", Page.Url.ToLower());

        await Page.GetByLabel("Email").FillAsync("suesmith@email.com");
        await Page.GetByLabel("Password").FillAsync("Pass123$");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's dashboard" }).IsVisibleAsync());

        await Page.Locator("li.nhsuk-card-group__item").Filter(new() { HasText = "orders" })
                  .GetByRole(AriaRole.Link, new() { Name = "View orders" })
                  .ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's orders" }).IsVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Create new order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start order" }).ClickAsync();
        await Page.GetByText("Catalogue solution and other").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Tech Innovation" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).FillAsync("Test Order");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Primary contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Primary contact details" }).ClickAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "First name" }).FillAsync("First name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" }).FillAsync("Last name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Telephone number" }).FillAsync("0121233654445");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).FillAsync("test@test.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();


        //Supplier information and contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Supplier information and" }).ClickAsync();

        await SelectAndConfirmSupplier(Page, "EMIS Health");
        await Page.GetByLabel("Yes").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm supplier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Supplier contact details" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Timescales for call-off agreement
        await Page.GetByRole(AriaRole.Link, new() { Name = "Timescales for call-off agreement" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Timescales for call-off agreement" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month", Exact = true }).FillAsync("04");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement initial period (in months)?" }).FillAsync("6");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement duration (in months)?" }).FillAsync("12");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 2
        //Service Recipients
        await Page.GetByRole(AriaRole.Link, new() { Name = "service recipients" }).ClickAsync();
        await Page.GetByLabel("Add service recipients manually").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Select sublocations for this order" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "02T" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BANKFIELD SURGERY" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm service recipients" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Solution and services
        await Page.GetByRole(AriaRole.Link, new() { Name = "Solutions and services" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solutions" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Emis Web GP" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Catalogue solution" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Row, new() { Name = "BANKFIELD SURGERY B84016" }).GetByLabel("Patient total").FillAsync("2");
        await Page.GetByRole(AriaRole.Row, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).GetByLabel("Patient total").FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Edit solutions and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add Associated services" }).ClickAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Engineering" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync(); await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Associated service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of associated service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BANKFIELD SURGERY" }).GetByRole(AriaRole.Textbox).FillAsync("2");
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BEECHWOOD MEDICAL CENTRE" }).GetByRole(AriaRole.Textbox).FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of associated service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Edit solutions and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Planned delivery date
        await Page.GetByRole(AriaRole.Link, new() { Name = "Planned delivery dates" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Planned delivery date" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month" }).FillAsync("06");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review planned delivery dates" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();


        //Select funding sources
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select funding sources" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "Emis Web GP" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Local funding" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "Engineering" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Local funding" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 3
        //Implementation and payment
        await Page.GetByRole(AriaRole.Link, new() { Name = "Implementation milestones and payment triggers" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Implementation milestones and payment triggers" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Associated service milestone
        await Page.GetByRole(AriaRole.Link, new() { Name = "Associated service milestones" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Associated service milestones and payment triggers" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Associated service requirements
        await Page.GetByRole(AriaRole.Link, new() { Name = "Associated service requirements" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Associated service requirements" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Data processing information
        await Page.GetByRole(AriaRole.Link, new() { Name = "Data processing information" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Data processing information" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Declaration
        await Page.GetByRole(AriaRole.Link, new() { Name = "Declaration" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Declaration" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "I understand and agree to the" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 4
        //Review and complete order
        await Page.GetByRole(AriaRole.Link, new() { Name = "Review and complete order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review and complete order" }).IsVisibleAsync();
    }

    [Fact]
    public async Task OrderWithCatalogueSolutionAndAdditionalService()
    {
        await Page.GotoAsync(Fixture.BaseUrl);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Log in" }).ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("login", Page.Url.ToLower());

        await Page.GetByLabel("Email").FillAsync("suesmith@email.com");
        await Page.GetByLabel("Password").FillAsync("Pass123$");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's dashboard" }).IsVisibleAsync());

        await Page.Locator("li.nhsuk-card-group__item").Filter(new() { HasText = "orders" })
                  .GetByRole(AriaRole.Link, new() { Name = "View orders" })
                  .ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's orders" }).IsVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Create new order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Start order" }).ClickAsync();
        await Page.GetByText("Catalogue solution and other").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Tech Innovation" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" }).FillAsync("Test Order");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Primary contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Primary contact details" }).ClickAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "First name" }).FillAsync("First name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" }).FillAsync("Last name");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Telephone number" }).FillAsync("0121233654445");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" }).FillAsync("test@test.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();


        //Supplier information and contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Supplier information and" }).ClickAsync();

        await SelectAndConfirmSupplier(Page, "EMIS Health");
        await Page.GetByLabel("Yes").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm supplier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Supplier contact details" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Timescales for call-off agreement
        await Page.GetByRole(AriaRole.Link, new() { Name = "Timescales for call-off agreement" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Timescales for call-off agreement" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month", Exact = true }).FillAsync("04");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement initial period (in months)?" }).FillAsync("6");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement duration (in months)?" }).FillAsync("12");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 2
        //Service Recipients
        await Page.GetByRole(AriaRole.Link, new() { Name = "service recipients" }).ClickAsync();
        await Page.GetByLabel("Add service recipients manually").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Select sublocations for this order" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "02T" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BANKFIELD SURGERY" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm sublocations" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm service recipients" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Solution and services
        await Page.GetByRole(AriaRole.Link, new() { Name = "Solutions and services" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solutions" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Emis Web GP" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Catalogue solution" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Row, new() { Name = "BANKFIELD SURGERY B84016" }).GetByLabel("Patient total").FillAsync("2");
        await Page.GetByRole(AriaRole.Row, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).GetByLabel("Patient total").FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Edit solutions and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add Additional services" }).ClickAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Automated Arrivals" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync(); await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Additional service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solution and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of additional service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BANKFIELD SURGERY" }).GetByRole(AriaRole.Textbox).FillAsync("2");
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BEECHWOOD MEDICAL CENTRE" }).GetByRole(AriaRole.Textbox).FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of additional service" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Edit solutions and services" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Planned delivery date
        await Page.GetByRole(AriaRole.Link, new() { Name = "Planned delivery dates" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Planned delivery date" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync("01");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month" }).FillAsync("06");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync("2026");
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review planned delivery dates" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();


        //Select funding sources
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select funding sources" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "Emis Web GP" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Local funding" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "Automated Arrivals" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Local funding" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Funding sources" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 3
        //Implementation and payment
        await Page.GetByRole(AriaRole.Link, new() { Name = "Implementation milestones and payment triggers" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Implementation milestones and payment triggers" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Data processing information
        await Page.GetByRole(AriaRole.Link, new() { Name = "Data processing information" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Data processing information" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Declaration
        await Page.GetByRole(AriaRole.Link, new() { Name = "Declaration" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Declaration" }).IsVisibleAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "I understand and agree to the" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Step 4
        //Review and complete order
        await Page.GetByRole(AriaRole.Link, new() { Name = "Review and complete order" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Review and complete order" }).IsVisibleAsync();
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
}
