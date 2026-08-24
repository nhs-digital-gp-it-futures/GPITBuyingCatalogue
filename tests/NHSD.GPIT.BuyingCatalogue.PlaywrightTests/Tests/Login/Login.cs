using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

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
    public async Task CompetitionPriceAndNonPriceJourney()
    {
        //await orderPages.LoginAsync();
        await Page.GotoAsync(Fixture.BaseUrl);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Log in" }).ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("login", Page.Url.ToLower());

        await Page.GetByLabel("Email").FillAsync("suesmith@email.com");
        await Page.GetByLabel("Password").FillAsync("Pass123$");
        await CompleteRecaptchaAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Competitions", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Your organisation's competitions" }).IsVisibleAsync());

        //create new competition setup
        await Page.GetByRole(AriaRole.Link, new() { Name = "Create new competition" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Before you create a competition" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create competition" }).ClickAsync();

        //select shortlist
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Select a shortlist" }).IsVisibleAsync());
        await Page.GetByLabel("Which shortlist do you want").SelectOptionAsync(new[] { "1" });
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Results for this shortlist" }).IsVisibleAsync());

        //Step 1
        //create new competition
        await Page.GetByRole(AriaRole.Link, new() { Name = "Create competition" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Create a competition" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Competition name" }).FillAsync($"Test {Random.Shared.Next(100, 1000)}");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Competition description" }).FillAsync("dsfgs");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        //create new competition - refine shortlist
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Refine shortlist for this competition" }).IsVisibleAsync());
        await SelectSolutionAsync("Emis Web GP");
        await SelectSolutionAsync("Write on Time");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //create new competition - excluded solutions
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Excluded solutions" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Why has this solution not" }).FillAsync("faasdfsa");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //create new competition - confirm shortlisted solutions
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm shortlisted solutions" }).IsVisibleAsync());
        await Page.GetByText("I want to continue with this").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm shortlist" }).ClickAsync();
        await Expect(Page.Locator("h1 + div.nhsuk-hint")).ToHaveTextAsync("Complete the following steps to carry out a competition.");

        //Service Recipients
        await Page.GetByRole(AriaRole.Link, new() { Name = "service recipients" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Service recipients" }).IsVisibleAsync());
        await Page.GetByLabel("Add service recipients manually").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Select sublocations for this competition" }).IsVisibleAsync());
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

        //Conract length
        await Page.GetByRole(AriaRole.Link, new() { Name = "Contract length" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Contract length" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "How long will your contract" }).FillAsync("18");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.Locator("h1 + div.nhsuk-hint")).ToHaveTextAsync("Complete the following steps to carry out a competition.");

        //Step 2 -- Define Competition criteria
        //Award criteria
        await Page.GetByRole(AriaRole.Link, new() { Name = "Award criteria" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "What criteria do you want to use to compare solutions?" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Price and non-price elements" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.Locator("h1 + div.nhsuk-hint")).ToHaveTextAsync("Complete the following steps to carry out a competition.");

        //Award criteria waiting

        await Page.GetByRole(AriaRole.Link, new() { Name = "Award criteria weightings" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "How would you like to weight your award criteria for this competition?" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Price weighting as a percentage", Exact = true }).FillAsync("70");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Non-price weighting as a" }).FillAsync("30");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

        //Non price elements
        //Non price elements - Add feature requirements
        await Page.GetByRole(AriaRole.Link, new() { Name = "Non-price elements" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add feature requirements" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Features requirements" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Must" }).CheckAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Feature requirement" }).FillAsync("dsfgsdf");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        //Non price elements - Add implementation requirements
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add implementation requirements" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Implementation requirements" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "What implementation" }).FillAsync("sdfgdsfg");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        //Add introperability requirements
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add interoperability requirements" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Interoperability requirements" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Bulk" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Access Record HTML" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Online Consultations" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        //service level requirements
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add service level requirements" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Service level requirements" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "From" }).FillAsync("09:00");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Until" }).FillAsync("18:00");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Monday" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Tuesday" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Wednesday" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Thursday" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Friday" }).CheckAsync();
        await Page.GetByText("Yes, include Bank Holidays").ClickAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes, include Bank Holidays" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Non-price elements" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //Non price weightings
        await Page.GetByRole(AriaRole.Link, new() { Name = "Non-price weightings" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "How would you like to weight your non-price elements for this competition?" }).IsVisibleAsync());        
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Features weighting as a" }).FillAsync("25");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Implementation weighting as a" }).FillAsync("25");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Interoperability weighting as" }).FillAsync("25");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Service level weighting as a" }).FillAsync("25");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        //Review competition criteria
        await Page.GetByRole(AriaRole.Link, new() { Name = "Review competition criteria" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Review competition criteria" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "I confirm I want proceed with" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm competition criteria" }).ClickAsync();

        //Step 3 -- Compare and score solutions
        //Compare and score non-price elements
        await Page.GetByRole(AriaRole.Link, new() { Name = "Compare and score non-price" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Compare and score shortlisted solutions" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Row, new() { Name = "Features" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Compare and score features" }).IsVisibleAsync());
        await EnterScoreAndJustificationAsync(0, "4", "Meets most of our requirements well.");
        await EnterScoreAndJustificationAsync(1, "3", "Adequate but limited customisation.");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Compare and score shortlisted solutions" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Row, new() { Name = "Implementation" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Compare and score implementation" }).IsVisibleAsync());
        await EnterScoreAndJustificationAsync(0, "4", "Meets most of our requirements well.");
        await EnterScoreAndJustificationAsync(1, "3", "Adequate but limited customisation.");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Compare and score shortlisted solutions" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Row, new() { Name = "Interoperability" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Compare and score interoperability" }).IsVisibleAsync());
        await EnterScoreAndJustificationAsync(0, "4", "Meets most of our requirements well.");
        await EnterScoreAndJustificationAsync(1, "3", "Adequate but limited customisation.");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Compare and score shortlisted solutions" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Row, new() { Name = "Service levels" }).GetByRole(AriaRole.Link, new() { Name = "Start" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Compare and score service levels" }).IsVisibleAsync());
        await EnterScoreAndJustificationAsync(0, "4", "Meets most of our requirements well.");
        await EnterScoreAndJustificationAsync(1, "3", "Adequate but limited customisation.");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Compare and score shortlisted solutions" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();


        //Calculate price
        await Page.GetByRole(AriaRole.Link, new() { Name = "Calculate price" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Calculate price" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).First.ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Price and quantity" }).IsVisibleAsync());


        //catalogue solution price and quantity- Emis Web GP
        await Page.Locator("#SolutionDetails").GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Catalogue solution" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Price and quantity" }).IsVisibleAsync());
        await Page.Locator("#SolutionDetails").GetByRole(AriaRole.Row, new() { Name = "Quantity" }).GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Review patient list sizes" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Row, new() { Name = "BANKFIELD SURGERY B84016" }).GetByLabel("Patient total").FillAsync("2");
        await Page.GetByRole(AriaRole.Row, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).GetByLabel("Patient total").FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Price and quantity" }).IsVisibleAsync();

        //additional service price and quantity
        await Page.Locator("#AdditionalServiceDetails").GetByRole(AriaRole.Row, new() { Name = "Price" }).GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Additional service" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.Locator("#AdditionalServiceDetails").GetByRole(AriaRole.Row, new() { Name = "Quantity" }).GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of additional service" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of additional service" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BANKFIELD SURGERY" }).GetByRole(AriaRole.Textbox).FillAsync("2");
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BEECHWOOD MEDICAL CENTRE" }).GetByRole(AriaRole.Textbox).FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of additional service" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        //catalogue solution price and quantity- Write on Time
        // Start for Write on Time
        await Page.GetByRole(AriaRole.Row, new() { Name = "Write on Time" }).GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Price and quantity" }).IsVisibleAsync());
        await Page.Locator("#SolutionDetails").GetByRole(AriaRole.Row, new() { Name = "Price" }).GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true }).ClickAsync();
        await Page.GetByLabel("Price 1: per patient").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Catalogue solution" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.Locator("#SolutionDetails").GetByRole(AriaRole.Row, new() { Name = "Quantity" }).GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Review patient list sizes" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Row, new() { Name = "BANKFIELD SURGERY B84016" }).GetByLabel("Patient total").FillAsync("2");
        await Page.GetByRole(AriaRole.Row, new() { Name = "BEECHWOOD MEDICAL CENTRE" }).GetByLabel("Patient total").FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of catalogue solution" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Price and quantity" }).IsVisibleAsync();

        //additional service price and quantity
        await Page.Locator("#AdditionalServiceDetails").GetByRole(AriaRole.Row, new() { Name = "Price" }).GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Price of Additional service" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        await Page.Locator("#AdditionalServiceDetails").GetByRole(AriaRole.Row, new() { Name = "Quantity" }).GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of additional service" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Review patient list sizes" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BANKFIELD SURGERY" }).GetByRole(AriaRole.Textbox).FillAsync("2");
        await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BEECHWOOD MEDICAL CENTRE" }).GetByRole(AriaRole.Textbox).FillAsync("20");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Quantity of additional service" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Confirm quantities" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Calculate price" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();
        await Expect(Page.Locator("h1 + div.nhsuk-hint")).ToHaveTextAsync("Complete the following steps to carry out a competition.");


        //Step 4- Finish and review results
        await Page.GetByRole(AriaRole.Link, new() { Name = "View results" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Are you ready to view the results for this competition?" }).IsVisibleAsync());
        await Page.GetByLabel("I confirm I want to complete the competition and view the results").CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "View results" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Heading, new() { Name = "Competition results" }).IsVisibleAsync());
    }

    public async Task EnterScoreAndJustificationAsync(int solutionIndex, string score, string justification)
    {
        await Page.Locator($"#SolutionScores_{solutionIndex}__Score").FillAsync(score);
        await Page.Locator($"#SolutionScores_{solutionIndex}__Justification").FillAsync(justification);
    }

    public async Task SelectSolutionAsync(string solutionName)
    {
        var nameAttribute = await Page.Locator($"input[value='{solutionName}'][name$='.SolutionName']")
            .GetAttributeAsync("id");

        if (nameAttribute is null)
        {
            throw new InvalidOperationException(
                $"Could not find solution '{solutionName}' on the page.");
        }

        // id is like "Solutions_1__SolutionName", extract "1"
        var index = nameAttribute.Split('_')[1];

        await Page.Locator($"#Solutions_{index}__Selected").CheckAsync();
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
}
